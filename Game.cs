using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace GGStat
{
    class Game {
        public static string[] Character = new string[]
        {
            "unknown",
            "sol",
            "ky",
            "millia",
            "zato",
            "may",
            "potemkin",
            "chipp",
            "venom",
            "axl",
            "ino",
            "faust",
            "slayer",
            "ramlethal",
            "bedman",
            "sin",
            "elphelt",
            "leo",
            "johnny",
            "jacko",
            "jam",
            "raven",
            "kum",
            "dizzy",
            "baiken",
            "answer"
        };

        private int[] playerCharacters = new int[2] { 0, 0 };

        private const string PROCESS_NAME = "GuiltyGearXrd";

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(int hProcess,
        Int64 lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

        public Dictionary<string, int> valueList = new Dictionary<string, int> {
            {"gameState", 4},
            {"timer", 99},
            {"player1Rounds", 0},
            {"player2Rounds", 0},
            {"player1HP", 420},
            {"player2HP", 420},
        };

        private Vision vision;
        private Memory memory;
        private Thread characterScanThread;

        public Dictionary<GameState, bool> gamestate = new Dictionary<GameState, bool> {
            { GameState.Loading, false },
            { GameState.MatchInProgress, false },
            { GameState.RoundInProgress, false },
            { GameState.InProgress, false },
            { GameState.Waiting, true }
        };
        
        public enum GameState {
            Loading,
            MatchInProgress,
            RoundInProgress,
            InProgress,
            Waiting,
        }

        public Game() {
            vision = new Vision(Program.process.MainWindowHandle);
            memory = new Memory();
        }

        public class Player
        {
            public string name;
            public int character;
            public int number;

            public Player(string name, int character, int number) {
                this.name = name;
                this.character = character;
                this.number = number;
            }
        }

        public class Match
        {
            public Player Winner {
                get {
                    int p1w = 0;
                    int p2w = 0;
                    foreach (Round round in rounds) {
                        if (round.winner == player1) p1w++;
                        if (round.winner == player2) p2w++;
                    }

                    return (p1w > p2w) ? player1 : player2;
                }
            }

            public Player player1;
            public Player player2;

            public List<Round> rounds = new List<Round>();

            public Match() {
            }

            public Match(Player player1, Player player2) {
                this.player1 = player1;
                this.player2 = player2;
            }

            override
            public string ToString() {
                return string.Format("{0}({1}) VS ({2}){3}",
                    player1.name, Character[player1.character], Character[player2.character], player2.name);
            }

            public void AddRound(int n, Round round) {
                rounds.Add(round);
                Console.WriteLine(round);
            }
        }

        public class Round
        {
            public Player winner;
            public Player loser;
            public int timeLeft;
            public int player1HPLeft;
            public int player2HPLeft;
            public int number;

            public Round(int n, Player winner, Player loser, int timeLeft, int player1HPLeft, int player2HPLeft) {
                this.number = n;
                this.winner = winner;
                this.loser = loser;
                this.timeLeft = timeLeft;
                this.player1HPLeft = player1HPLeft;
                this.player2HPLeft = player2HPLeft;
            }

            override
            public string ToString() {
                return string.Format("Round {0}! Winner: {1}({2}), Loser: {3}({4}). Time left {5}; p1 HP:{6}; p2 HP:{7}",
                    number, winner.name, Character[winner.character], loser.name, Character[loser.character], timeLeft, player1HPLeft, player2HPLeft);
            }
        }

        private int lastTimerValue = 99;

        Match match = null;

        Player p1;
        Player p2;

        public void Run() {
            Thread.Sleep(1000);
            memory.ScanMemory(ref valueList);
            lastTimerValue = valueList["timer"];
            Thread.Sleep(1000);
            do {
                Thread.Sleep(1000);
                //Console.Clear();
                memory.ScanMemory(ref valueList);
                
                foreach (KeyValuePair<string, int> item in valueList) {
                    Console.WriteLine(item.Key + ":" + item.Value);
                }

                if (lastTimerValue != valueList["timer"]) {
                    gamestate[GameState.MatchInProgress] = true;
                    gamestate[GameState.RoundInProgress] = true;
                } else {
                    Console.WriteLine("Match not in progress or game is paused");
                    gamestate[GameState.RoundInProgress] = false;

                    if (match != null) {
                        int round = valueList["player1Rounds"] + valueList["player2Rounds"];
                        if (round > 0 && match.rounds.Count < round && (valueList["gameState"] == 0 || valueList["gameState"] == 1)) {
                            var winner = (valueList["gameState"] == 0) ? p1 : p2;
                            var loser = (valueList["gameState"] == 0) ? p1 : p2;
                            match.AddRound(round, new Round(round,
                                    (valueList["gameState"] == 0) ? p1 : p2,
                                    (valueList["gameState"] == 0) ? p2 : p1,
                                    valueList["timer"],
                                    valueList["player1HP"], 
                                    valueList["player2HP"])
                                    );
                        } else if (valueList["player1Rounds"] == 2 || valueList["player2Rounds"] == 2) {
                            // GAME OVER
                            Console.WriteLine("-------------------------------------------");
                            Console.WriteLine(string.Format("Match over! Winner: {0}({1})", match.Winner.name, Character[match.Winner.character]));
                            foreach (var r in match.rounds) {
                                if (r != null) {
                                    Console.WriteLine(r);
                                }
                            }

                            Data.saveMatch(match);
                            Console.WriteLine("Match saved");
                            match = null;
                        }
                    }
                    continue;
                }

                Console.WriteLine(match);
                if (!gamestate[GameState.MatchInProgress] && valueList["timer"] == 0 && valueList["gameState"] == 4) {
                    Console.WriteLine("We are not in game at the moment...");
                    gamestate[GameState.MatchInProgress] = false;
                    gamestate[GameState.RoundInProgress] = false;
                    continue;
                } else
                // if the current round was won by a player, if the player HPs are reseted
                // and if either of the player round wins is 2, the match has with no doubt ended
                if (!gamestate[GameState.MatchInProgress] && (valueList["gameState"] == 0 || valueList["gameState"] == 1) &&
                    valueList["player1HP"] == 420 && valueList["player2HP"] == 420 &&
                    (valueList["player1Rounds"] == 2 || valueList["player2Rounds"] == 2)) {
                    Console.WriteLine("We are not in game at the moment...");
                    // if the match is not in progress, neither is the round
                    gamestate[GameState.MatchInProgress] = false;
                    gamestate[GameState.RoundInProgress] = false;

                    continue;
                }
                // if the round is not won by anyone yet and healths are at maximum and both
                // players have 0 round wins, the match has just started
                else if (valueList["gameState"] == 4 &&
                    valueList["player1HP"] == 420 && valueList["player2HP"] == 420 &&
                    valueList["player1Rounds"] == 0 && valueList["player2Rounds"] == 0) {
                    gamestate[GameState.MatchInProgress] = true;
                }

                if (gamestate[GameState.MatchInProgress] && match == null && p1 != null && p2 != null) {
                    match = new Match(p1, p2);
                }

                Console.WriteLine("MatchInProgress: " + gamestate[GameState.MatchInProgress]);
                Console.WriteLine("RoundInProgress: " + gamestate[GameState.RoundInProgress]);


                // So if the match is in progress and timer is at 99
                // Start scan threads when match starts
                if (gamestate[GameState.MatchInProgress]) {
                    if (playerCharacters[0] == 0 && playerCharacters[1] == 0 && characterScanThread == null) {
                        Console.WriteLine("start Character Scan Thread ");
                        characterScanThread = new Thread(() => CharacterThreadLoop());
                        characterScanThread.Start();
                    }
                }
                lastTimerValue = valueList["timer"];
            } while (true);
        }
        

        public void CharacterThreadLoop() {
            bool done = false;
            do {
                done = vision.FindPlayers(ref playerCharacters);
                if (done) { 
                    p1 = new Player("Player1", playerCharacters[0], 0);
                    p2 = new Player("Player2", playerCharacters[1], 1);
                }
                Thread.Sleep(2000);
            } while (!done);
        }
    }
}
