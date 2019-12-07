using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Data;
using System.Data.SQLite;

namespace GGStat {
    class Data {
        public static string dbFileName = "db.sqlite";
        public static SQLiteConnection connection;

        public static string db_lastInsert = "SELECT last_insert_rowid() as id";
        public static string db_tableList = "SELECT name FROM sqlite_master WHERE type='table';";
        public static string db_createCharacterTable = "CREATE TABLE character ( id INTEGER PRIMARY KEY AUTOINCREMENT, name STRING UNIQUE, name_short STRING UNIQUE);";
        public static string db_createMatchTable = "CREATE TABLE match (id INTEGER PRIMARY KEY AUTOINCREMENT, winner INTEGER, player1_id INTEGER REFERENCES [player] (id) NOT NULL, player2_id INTEGER REFERENCES [player] (id) NOT NULL, player1_character_id INTEGER REFERENCES character (id), player2_character_id INTEGER REFERENCES character (id), timestamp TIMESTAMP DEFAULT (strftime('%s', 'now'))); CREATE UNIQUE INDEX idx_match_id ON match(id);";
        public static string db_createRoundTable = "CREATE TABLE round (id INTEGER PRIMARY KEY AUTOINCREMENT, match_id INTEGER REFERENCES [match] (id) NOT NULL, number INTEGER, winner INTEGER NOT NULL, time_left INTEGER DEFAULT (99) NOT NULL, player1_hp INTEGER DEFAULT (420) NOT NULL, player2_hp INTEGER DEFAULT (420) NOT NULL, timestamp TIMESTAMP DEFAULT (strftime('%s', 'now'))); CREATE UNIQUE INDEX idx_round_id ON round(id);";
        public static string db_createPlayerTable = "CREATE TABLE player (id INTEGER PRIMARY KEY AUTOINCREMENT, steamid INTEGER NOT NULL, UNIQUE(steamid) ON CONFLICT REPLACE);";
        public static string db_createAliasTable = "CREATE TABLE alias (id INTEGER PRIMARY KEY AUTOINCREMENT, name STRING NOT NULL, player_id INTEGER REFERENCES [player] (id) NOT NULL, timestamp DATETIME DEFAULT (strftime('%s', 'now')), UNIQUE(name, player_id) ON CONFLICT REPLACE);";
        public static string db_characterData = "INSERT INTO character(name, name_short) VALUES('Sol', 'SO'); INSERT INTO character(name, name_short) VALUES('Ky', 'KY'); INSERT INTO character(name, name_short) VALUES('Millia', 'MI'); INSERT INTO character(name, name_short) VALUES('Zato=1', 'ZA'); INSERT INTO character(name, name_short) VALUES('May', 'MA'); INSERT INTO character(name, name_short) VALUES('Potemkin', 'PO'); INSERT INTO character(name, name_short) VALUES('Chipp', 'CH'); INSERT INTO character(name, name_short) VALUES('Venom', 'VE'); INSERT INTO character(name, name_short) VALUES('Axl', 'AX'); INSERT INTO character(name, name_short) VALUES('I-No', 'IN'); INSERT INTO character(name, name_short) VALUES('Faust', 'FA'); INSERT INTO character(name, name_short) VALUES('Slayer', 'SL'); INSERT INTO character(name, name_short) VALUES('Ramlethal', 'RA'); INSERT INTO character(name, name_short) VALUES('Bedman', 'BE'); INSERT INTO character(name, name_short) VALUES('Sin', 'SI'); INSERT INTO character(name, name_short) VALUES('Elphelt', 'EL'); INSERT INTO character(name, name_short) VALUES('Leo', 'LE'); INSERT INTO character(name, name_short) VALUES('Johnny', 'JO'); INSERT INTO character(name, name_short) VALUES('Jack-o', 'JC'); INSERT INTO character(name, name_short) VALUES('Jam', 'JA'); INSERT INTO character(name, name_short) VALUES('Raven', 'RV'); INSERT INTO character(name, name_short) VALUES('Kum', 'KU'); INSERT INTO character(name, name_short) VALUES('Dizzy', 'DI'); INSERT INTO character(name, name_short) VALUES('Baiken', 'BA'); INSERT INTO character(name, name_short) VALUES('Answer', 'AN');";

        public static string db_saveMatch = "INSERT INTO match(winner, player1_id, player2_id, player1_character_id, player2_character_id) VALUES({0}, {1}, {2}, {3}, {4});";
        public static string db_saveRound = "INSERT INTO round(match_id, number, winner, time_left, player1_hp, player2_hp) VALUES({0}, {1}, {2}, {3}, {4}, {5});";
        public static string db_savePlayer = "INSERT INTO player(steamid) VALUES('{0}');";
        public static string db_saveAlias = "INSERT INTO alias(name, player_id) VALUES('{0}', {1});";

        public static string[] Character = new string[]
        {
            "N/A", // 0 
            "Sol", // 0 (This starts from 1 in the database) 
            "Ky", // 1
            "May", // 2
            "Millia", // 3
            "Zato=1", // 4
            "Potemkin", // 5
            "Chipp", // 6
            "Faust", // 7
            "Axl", // 8
            "Venom", // 9
            "Slayer", // 10
            "Ino", // 11
            "Bedman", // 12
            "Ramlethal", // 13
            "Sin", // 14
            "Elphelt", // 15
            "Leo", // 16
            "Johnny", // 17
            "Jack-O", // 18
            "Jam", // 19
            "Kum", // 20
            "Raven", // 21
            "Dizzy", // 22
            "Baiken", // 23
            "Answer" // 24
        };

        public class Match {
            public long id { get; set; }
            public List<Round> rounds { get; set; }
            public int winner { get; set; }
            public Player player1 { get; set; }
            public Player player2 { get; set; }
            public int player1_character_id { get; set; }
            public int player2_character_id { get; set; }
            public long timestamp { get; set; }

            public Match(long id, List<Round> rounds, int winner, Player player1, Player player2, int player1_character_id, int player2_character_id, long timestamp) {
                this.id = id;
                this.rounds = rounds;
                this.winner = winner;
                this.player1 = player1;
                this.player2 = player2;
                this.player1_character_id = player1_character_id;
                this.player2_character_id = player2_character_id;
                this.timestamp = timestamp;
            }
            public Match(long id, int winner, int player1, int player2, int player1_character_id, int player2_character_id, long timestamp) {
                this.id = id;
                this.rounds = rounds;
                this.winner = winner;
                this.player1 = new Player(player1);
                this.player2 = new Player(player2);
                this.player1_character_id = player1_character_id;
                this.player2_character_id = player2_character_id;
                this.timestamp = timestamp;
            }
        }

        public class Round {
            public int id { get; set; }
            public Match match { get; set; }
            public int number { get; set; }
            public int winner { get; set; }
            public int time_left { get; set; }
            public int player1_hp { get; set; }
            public int player2_hp { get; set; }
            public long timestamp { get; set; }

            public Round(int id, Match match, int number, int winner, int time_left, int player1_hp, int player2_hp, long timestamp) {
                this.id = id;
                this.match = match;
                this.number = number;
                this.winner = winner;
                this.time_left = time_left;
                this.player1_hp = player1_hp;
                this.player2_hp = player2_hp;
                this.timestamp = timestamp;
            }
        }

        public class Player {
            public long id { get; set; }
            public long steamId { get; set; }
            public List<string> aliases { get; set; }
            public string latestAlias { get; set; }
            public Player(long id, long steamId, List<string> aliases, string latestAlias) {
                this.id = id;
                this.steamId = steamId;
                this.aliases = aliases;
                this.latestAlias = latestAlias;
            }
            public Player(long id, long steamId, string aliasString) {
                this.id = id;
                this.steamId = steamId;

                string[] aliasArr = aliasString.ToString().Split(new[] { "|!|" }, StringSplitOptions.None);
                string temp;
                for (int i = 0; i < aliasArr.Length; i++) {
                    for (int j = 0; j < aliasArr.Length; j++) {
                        if (int.Parse(aliasArr[i].Split(new[] { "|:|" }, StringSplitOptions.None)[1]) < int.Parse(aliasArr[j].Split(new[] { "|:|" }, StringSplitOptions.None)[1])) {
                            temp = aliasArr[i];
                            aliasArr[i] = aliasArr[j];
                            aliasArr[j] = temp;
                        }
                    }
                }
                for (int i = 0; i < aliasArr.Length; i++) {
                    aliasArr[i] = aliasArr[i].Split(new[] { "|:|" }, StringSplitOptions.None)[0];
                }
                this.aliases = new List<string>(aliasArr);
                this.latestAlias = aliases[0];
            }

            public Player(int id) {
                this.id = id;
            }

            override public string ToString() {
                int aliasCount = ((aliases.Count > 3) ? 3 : aliases.Count);
                string alias = "";
                for (int i = 0; i < aliasCount; i++) {
                    string a = aliases[i];
                    if (i == 0) {
                        alias = a;
                    } else {
                        if (i == 1) {
                            alias += "  ( " + a;
                        } else {
                            alias += ", " + a;
                        }
                        if (i == aliasCount - 1) {
                            alias += " )";
                        }
                    }
                }
                return alias;
            }
        }

        public static SQLiteConnection Connect() {
            if (connection == null || connection.State != ConnectionState.Open) {
                connection = new SQLiteConnection("Data Source=" + dbFileName + ";Version=3;datetimeformat=CurrentCulture");
                connection.Open();
            }
            return connection;
        }

        public static void Disconnect() {
            connection.Close();
        }

        public static void InitDB() {
            createDatabase();
            addCharacterData();
        }

        public static void saveMatch(Game.Match match) {
            int player1ID = SavePlayer(match.player1.steamId, match.player1.name);
            if (player1ID < 0) return;
            int player2ID = SavePlayer(match.player2.steamId, match.player2.name);
            if (player2ID < 0) return;

            string sql = string.Format(db_saveMatch, match.Winner.seatId, player1ID, player2ID, match.player1.character, match.player2.name);

            SQLiteTransaction transaction = BeginTransaction();
            try {
                NonQuery(sql);

                int matchID = (int)connection.LastInsertRowId;

                foreach (Game.Round r in match.rounds) {
                    sql = string.Format(db_saveRound, matchID, r.number, r.winner.seatId, r.timeLeft, r.player1HPLeft, r.player2HPLeft);
                    NonQuery(sql);
                }
                transaction.Commit();
            } catch (Exception) {
                transaction.Rollback();
            }
        }

        private static void addCharacterData() {
            if (SelectCharacters().Count == 0) {
                SQLiteTransaction transaction = BeginTransaction();
                try {
                    NonQuery(db_characterData);
                    transaction.Commit();
                } catch (Exception) {
                    transaction.Rollback();
                }
            }
        }

        private static void createDatabase() {
            string executableLocation = Path.GetDirectoryName(
                Assembly.GetExecutingAssembly().Location);
            string dbLocation = Path.Combine(executableLocation, dbFileName);
            Dictionary<string, string> neededTables = new Dictionary<string, string> { { "character", db_createCharacterTable },
                { "round", db_createRoundTable },
                { "match", db_createMatchTable },
                { "player", db_createPlayerTable },
                { "alias", db_createAliasTable }
            };

            if (!File.Exists(dbLocation)) {
                SQLiteConnection.CreateFile(dbFileName);
                SQLiteTransaction transaction = BeginTransaction();
                try {
                    foreach (var nt in neededTables) {
                        NonQuery(nt.Value);
                    }
                    transaction.Commit();
                } catch (Exception) {
                    transaction.Rollback();
                }
                Program.Log("DB file created");
            } else {
                Program.Log("DB file already exists");
                List<string> tables = GetTableList();

                SQLiteTransaction transaction = BeginTransaction();
                try {
                    foreach (var nt in neededTables) {
                        bool f = false;
                        foreach (string t in tables) {
                            if (nt.Key == t) {
                                f = true;
                                break;
                            }
                        }
                        // create new table if not found
                        if (!f) {
                            NonQuery(nt.Value);
                        }
                    }
                    transaction.Commit();
                } catch (Exception) {
                    transaction.Rollback();
                }
            }
        }

        private static List<string[]> SelectCharacters() {
            SQLiteDataReader reader = ReaderQuery("select * from character");

            List<string[]> res = new List<string[]>();
            while (reader.Read()) {
                if (Program.DEBUG) Program.Log(reader["id"].ToString() + ", " + (string)reader["name"] + ", " + (string)reader["name_short"]);
                res.Add(new string[] { reader["id"].ToString(), (string)reader["name"], (string)reader["name_short"] });
            }

            return res;
        }

        public static int SavePlayer(long steamID, string alias) {
            SQLiteTransaction transaction = BeginTransaction();
            try {
                // Save Player, override on conflict
                string sql = string.Format(db_savePlayer, steamID);
                NonQuery(sql);

                transaction.Commit();
                int playerID = (int)connection.LastInsertRowId;

                try {
                    // Save the alias, override on conflict
                    sql = string.Format(db_saveAlias, alias, playerID);
                    NonQuery(sql);
                    return playerID;
                } catch (Exception) {
                    transaction.Rollback();
                    NonQuery(string.Format("delete from player where id = {0}", playerID));
                    return -1;
                }
            } catch (Exception) {
                transaction.Rollback();
                return -1;
            }
        }

        private static List<string> GetTableList() {
            SQLiteDataReader reader = ReaderQuery(db_tableList);
            List<string> tableList = new List<string>();
            while (reader.Read()) {
                tableList.Add((string)reader["name"]);
            }

            return tableList;
        }

        public static void NonQuery(string q) {
            Connect();
            SQLiteCommand command = new SQLiteCommand(q, connection);
            command.ExecuteNonQuery();
        }
        public static SQLiteDataReader ReaderQuery(string q) {
            Connect();
            try {
                SQLiteCommand command = new SQLiteCommand(q, connection);
                SQLiteDataReader reader = command.ExecuteReader();
                return reader;
            } catch (Exception) {
                throw;
            }
        }

        public static SQLiteTransaction BeginTransaction() {
            Connect();
            return connection.BeginTransaction();
        }

        /*
         * FOR UI DATA PURPOSES
         */
        public static Player GetPlayerBySteamId(long steamId) {
            string sql = string.Format("select p.*, group_concat(alias.name || '|:|' || alias.timestamp,'|!|') AS aliases FROM player p INNER JOIN alias ON alias.player_id = p.id WHERE steamid = {0}", steamId);
            SQLiteDataReader r = ReaderQuery(sql);
            r.Read();

            if (r["id"] != null) {
                return new Player((long)r["id"], (long)r["steamid"], r["aliases"].ToString());
            } else {
                return null;
            }
        }

        public static List<Player> getAllPlayers() {
            SQLiteDataReader r = ReaderQuery("SELECT p.*, group_concat(alias.name || '|:|' || alias.timestamp,'|!|') AS aliases FROM player p INNER JOIN alias ON alias.player_id = p.id GROUP BY p.id;");
            List<Player> arr = new List<Player>();
            while (r.Read()) {
                arr.Add(new Player((long)r["id"], (long)r["steamid"], r["aliases"].ToString()));
            }
            return arr;
        }

        public static List<Match> x(long steamID) {
            SQLiteDataReader r = ReaderQuery(string.Format("SELECT m.* FROM match m WHERE EXISTS (SELECT id FROM player WHERE steamid = {0} AND (id = m.player1_id OR id = m.player2_id ));", steamID));
            List<Match> arr = new List<Match>();
            while (r.Read()) {
                arr.Add(new Match((long)r["id"], (int)r["winner"], (int)r["player1_id"], (int)r["player2_id"], (int)r["player1_character_id"], (int)r["player2_character_id"], (long)r["timestamp"]));
            }
            return arr;
        }

        public static List<Match> getAllMatchesWithPlayer(Player player) {
            SQLiteDataReader r = ReaderQuery(string.Format("SELECT * FROM match WHERE player1_id = {0} OR player2_id = {0};", player.id));
            List<Match> arr = new List<Match>();
            while (r.Read()) {
                long id = (long)r["id"];
                int winner = int.Parse(r["winner"].ToString());
                int player1_id = int.Parse(r["player1_id"].ToString());
                int player2_id = int.Parse(r["player2_id"].ToString());
                int player1_character_id = int.Parse(r["player1_character_id"].ToString());
                int player2_character_id = int.Parse(r["player2_character_id"].ToString());
                long timestamp = r.GetInt64(6);
                arr.Add(new Match(id, winner, player1_id, player2_id, player1_character_id, player2_character_id, timestamp));
            }
            return arr;
        }

        public static void getAllMatchesWithCharacterOnPlayer(long steamID, int characterID) {
            ReaderQuery(string.Format("SELECT * from match m INNER JOIN player p1 ON p1.id = player1_id INNER JOIN player p2 ON p2.id = player2_id WHERE (p1.steamid = {0} AND m.player1_character_id = {1}) OR (p2.steamid = {0} AND m.player2_character_id = {1});", steamID, characterID));
        }
        public static void getAllMatchesWithCharacterOnPlayer(Player player, int characterID) {
            ReaderQuery(string.Format("SELECT * FROM match WHERE (player1_id = {0} AND player1_character_id = {1}) OR (player2_id = {0} AND player2_character_id = {1});", player.id, characterID));
        }
        public static void getAllMatchesBetweenPlayers(long player1SteamID, long player2SteamID) {
            ReaderQuery(string.Format("SELECT m.* FROM match m INNER JOIN player p1 ON p1.id = player1_id INNER JOIN player p2 ON p2.id = player2_id WHERE (p1.steamid, p2.steamid) IN (VALUES ({0},{1}), ({1}, {0}));", player1SteamID, player2SteamID));
        }
        public static void getAllMatchesBetweenPlayers(Player player1, Player player2) {
            ReaderQuery(string.Format("SELECT * FROM match  WHERE (player1_id, player2_id ) IN (VALUES ({0},{1}), ({1}, {0}));", player1.id, player2.id));
        }
    }
}
