using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace GGStat {
    class Game {


        private const string PROCESS_NAME = "GuiltyGearXrd";

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(int hProcess,
        Int64 lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

        private Memory memory;

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
            memory = new Memory();
        }

        public class Player {
            public long steamId { get; set; }
            public string name { get; set; }
            public int character { get; set; }
            public int matchesWon { get; set; }
            public int matchesSum { get; set; }
            public int loadingPct { get; set; }
            public int cabId { get; set; }
            public int seatId { get; set; }

            public Player(long steamId, string name, int character, int matchesWon, int matchesSum, int loadingPct, int cabId, int seatId) {
                this.steamId = steamId;
                this.name = name;
                this.character = character;
                this.matchesWon = matchesWon;
                this.matchesSum = matchesSum;
                this.loadingPct = loadingPct;
                this.cabId = cabId;
                this.seatId = seatId;
            }

            public bool isValid() { return steamId > 0; }

            public string toString() {
                return "steamId: " + steamId +
                    ", name: " + name +
                    ", character: " + Data.Character[character] +
                    ", matchesWon: " + matchesWon +
                    ", matchesSum: " + matchesSum +
                    ", loadingPct: " + loadingPct +
                    ", cabId: " + cabId +
                    ", seatId: " + seatId;
            }
        }

        public class MatchState {
            public int timer { get; set; }
            public int[] health { get; set; }
            public int[] rounds { get; set; }
            public int[] tension { get; set; }
            public bool[] hasBurst { get; set; }
            public int[] risc { get; set; }
            public int[] stun { get; set; }
            public int state { get; set; }

            public MatchState() { }
            public MatchState(int timer, int[] health, int[] rounds, int[] tension, bool[] hasBurst, int[] risc, int[] stun, int state) {
                this.timer = timer;
                this.health = health;
                this.rounds = rounds;
                this.tension = tension;
                this.hasBurst = hasBurst;
                this.risc = risc;
                this.stun = stun;
                this.state = state;
            }
            public bool isValid() { return timer > 0; }
            public string toString() {
                if (!isValid()) return "MatchState Not Valid";

                return "timer: " + timer +
                    ", health_1: " + health[0] + ", health_2: " + health[1] +
                    ", rounds_1: " + rounds[0] + ", rounds_2: " + rounds[1] +
                    ", tension_1: " + tension[0] + ", tension_2: " + tension[1] +
                    ", hasBurst_1: " + hasBurst[0] + ", hasBurst_2: " + hasBurst[1] +
                    ", risc_1: " + risc[0] + ", risc_2: " + risc[1] +
                    ", stun_1: " + stun[0] + ", stun_2: " + stun[1] +
                    ", state: " + state;
            }
        }

        public class Match {
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
                    player1.name, Data.Character[player1.character], Data.Character[player2.character], player2.name);
            }

            public void AddRound(int n, Round round) {
                rounds.Add(round);
                Program.Log(round.ToString());
            }
        }

        public class Round {
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
                    number, winner.name, Data.Character[winner.character], loser.name, Data.Character[loser.character], timeLeft, player1HPLeft, player2HPLeft);
            }
        }

        Match match = null;
        Player p1;
        Player p2;
        Player client;

        public void Run() {
            List<Player> playerList = memory.getPlayerData();
            MatchState state = null;
            Program.Log("Waiting...");
            do {

                int validPlayers = 0;
                do {
                    validPlayers = 0;
                    playerList = memory.getPlayerData();
                    foreach (Player player in playerList) {
                        if (player.isValid()) { validPlayers++; }
                    }
                    Console.Write(".");
                    System.Threading.Thread.Sleep(100);
                } while (validPlayers < 2);

                // Find client from player list.
                foreach (Player player in playerList) {
                    if (player.steamId == memory.getClientSteamId()) {
                        client = player;
                        break;
                    }
                }

                state = memory.getMatchState();
                // Lets find all players on the same cab as the client, including spectators.
                foreach (Player player in playerList) {
                    if (client.cabId == player.cabId) {
                        if (p1 != null && p2 != null) break;
                        if (player.isValid()) {
                            if (player.seatId == 0) {
                                p1 = player;
                                continue;
                            } else if (player.seatId == 1) {
                                p2 = player;
                                continue;
                            }
                        }
                    }
                }

                if (p1 == null || !p1.isValid() || p2 == null || !p2.isValid()) {
                    // players are not valid...
                    continue;
                }
                Program.Log(".");

                // Wait until we recognize a running match
                state = memory.getMatchState();
            } while (!state.isValid());

            // MATCH START!
            if (state.rounds[0] == 0 && state.rounds[1] == 0 && state.timer >= 98 && state.health[0] == 420 && state.health[1] == 420) {
                Program.Log("New Match starting!");
                Program.Log("Player 1: " + p1.toString());
                Program.Log("Player 2: " + p2.toString());
                match = new Match(p1, p2);
            } else {
                // the match was most likely already going on when joining to spectate? lets break out of this.
                Program.Log("Match was already running when joining to spectate, or Match not ready for tracking yet.");
                System.Threading.Thread.Sleep(500);
                return;
            }

            // MATCH IS RUNNING HERE
            do {
                if (Program.DEBUG) Program.Log(state.toString());
                // ROUND END!
                int round = state.rounds[0] + state.rounds[1];
                if (round > 0 && match.rounds.Count < round && (state.state == 0 || state.state == 1)) {
                    var winner = (state.state == 0) ? p1 : p2;
                    var loser = (state.state == 1) ? p1 : p2;
                    match.AddRound(round, new Round(round,
                            winner,
                            loser,
                            state.timer,
                            state.health[0],
                            state.health[1])
                            );
                } else if (state.rounds[0] == 2 || state.rounds[1] == 2) {
                    // GAME OVER
                    Program.Log("-------------------------------------------");
                    Program.Log(string.Format("Match over! Winner: {0}({1})", match.Winner.name, Data.Character[match.Winner.character]));
                    foreach (var r in match.rounds) {
                        if (r != null) {
                            Program.Log(r.ToString());
                        }
                    }

                    Data.saveMatch(match);
                    Program.Log("Match saved");
                    match = null;
                }

                state = memory.getMatchState();
                System.Threading.Thread.Sleep(1000);
            } while (match != null && state.isValid());

            /*
            Program.Log(match);
            if (!gamestate[GameState.MatchInProgress] && valueList["timer"] == 0 && valueList["gameState"] == 4)
            {
                Program.Log("We are not in game at the moment...");
                gamestate[GameState.MatchInProgress] = false;
                gamestate[GameState.RoundInProgress] = false;
                continue;
            }
            else
            // if the current round was won by a player, if the player HPs are reseted
            // and if either of the player round wins is 2, the match has with no doubt ended
            if (!gamestate[GameState.MatchInProgress] && (valueList["gameState"] == 0 || valueList["gameState"] == 1) &&
                valueList["player1HP"] == 420 && valueList["player2HP"] == 420 &&
                (valueList["player1Rounds"] == 2 || valueList["player2Rounds"] == 2))
            {
                Program.Log("We are not in game at the moment...");
                // if the match is not in progress, neither is the round
                gamestate[GameState.MatchInProgress] = false;
                gamestate[GameState.RoundInProgress] = false;

                continue;
            }
            // if the round is not won by anyone yet and healths are at maximum and both
            // players have 0 round wins, the match has just started
            else if (valueList["gameState"] == 4 &&
                valueList["player1HP"] == 420 && valueList["player2HP"] == 420 &&
                valueList["player1Rounds"] == 0 && valueList["player2Rounds"] == 0)
            {
                gamestate[GameState.MatchInProgress] = true;
            }

            if (gamestate[GameState.MatchInProgress] && match == null && p1 != null && p2 != null && valueList["ownerPosition"] == 0 || valueList["ownerPosition"] == 1)
            {
                match = new Match(p1, p2, valueList["ownerPosition"]);
            }

            Program.Log("MatchInProgress: " + gamestate[GameState.MatchInProgress]);
            Program.Log("RoundInProgress: " + gamestate[GameState.RoundInProgress]);


            // So if the match is in progress and timer is at 99
            // Start scan threads when match starts
            if (gamestate[GameState.MatchInProgress])
            {
                if (playerCharacters[0] == 0 && playerCharacters[1] == 0 && characterScanThread == null)
                {
                    Program.Log("start Character Scan Thread ");
                    characterScanThread = new Thread(() => CharacterThreadLoop());
                    characterScanThread.Start();
                }
            }
            lastTimerValue = valueList["timer"];*/
        }
    }
}
