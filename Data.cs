using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;
using System.Reflection;
using System.Data;

namespace GGStat
{
    class Data
    {
        public static string dbFileName = "db.sqlite";
        public static SQLiteConnection connection;

        private static string db_lastInsert = "SELECT last_insert_rowid() as id";
        private static string db_tableList = "SELECT name FROM sqlite_master WHERE type='table';";
        private static string db_createCharacterTable = "CREATE TABLE character ( id INTEGER PRIMARY KEY AUTOINCREMENT, name STRING UNIQUE, name_short STRING UNIQUE);";
        private static string db_createMatchTable = "CREATE TABLE [match] (id INTEGER PRIMARY KEY AUTOINCREMENT, winner INTEGER, player1 STRING NOT NULL DEFAULT Player1, player1_character_id INTEGER DEFAULT (0) REFERENCES character (id), player2 STRING DEFAULT Player2NOT NULL, player2_character_id INTEGER REFERENCES character (id) DEFAULT (0));";
        private static string db_createRoundTable = "CREATE TABLE round (id INTEGER PRIMARY KEY AUTOINCREMENT, match_id INTEGER REFERENCES [match] (id) NOT NULL, number INTEGER, winner INTEGER NOT NULL, time_left INTEGER DEFAULT (99) NOT NULL, player1_hp INTEGER DEFAULT (420) NOT NULL, player2_hp INTEGER DEFAULT (420) NOT NULL);";
        private static string db_characterData = "INSERT INTO character(name, name_short) VALUES('Unknown', 'UN'); INSERT INTO character(name, name_short) VALUES('Sol', 'SO'); INSERT INTO character(name, name_short) VALUES('Ky', 'KY'); INSERT INTO character(name, name_short) VALUES('Millia', 'MI'); INSERT INTO character(name, name_short) VALUES('Zato=1', 'ZA'); INSERT INTO character(name, name_short) VALUES('May', 'MA'); INSERT INTO character(name, name_short) VALUES('Potemkin', 'PO'); INSERT INTO character(name, name_short) VALUES('Chipp', 'CH'); INSERT INTO character(name, name_short) VALUES('Venom', 'VE'); INSERT INTO character(name, name_short) VALUES('Axl', 'AX'); INSERT INTO character(name, name_short) VALUES('I-No', 'IN'); INSERT INTO character(name, name_short) VALUES('Faust', 'FA'); INSERT INTO character(name, name_short) VALUES('Slayer', 'SL'); INSERT INTO character(name, name_short) VALUES('Ramlethal', 'RA'); INSERT INTO character(name, name_short) VALUES('Bedman', 'BE'); INSERT INTO character(name, name_short) VALUES('Sin', 'SI'); INSERT INTO character(name, name_short) VALUES('Elphelt', 'EL'); INSERT INTO character(name, name_short) VALUES('Leo', 'LE'); INSERT INTO character(name, name_short) VALUES('Johnny', 'JO'); INSERT INTO character(name, name_short) VALUES('Jack-o', 'JC'); INSERT INTO character(name, name_short) VALUES('Jam', 'JA'); INSERT INTO character(name, name_short) VALUES('Raven', 'RV'); INSERT INTO character(name, name_short) VALUES('Kum', 'KU'); INSERT INTO character(name, name_short) VALUES('Dizzy', 'DI'); INSERT INTO character(name, name_short) VALUES('Baiken', 'BA'); INSERT INTO character(name, name_short) VALUES('Answer', 'AN');";
        private static string db_saveMatch = "INSERT INTO match(winner, player1, player1_character_id, player2, player2_character_id) ";
        private static string db_saveRound = "INSERT INTO round(match_id, number, winner, time_left, player1_hp, player2_hp) ";

        private static void connect() {
            connection = new SQLiteConnection("Data Source=" + dbFileName + ";Version=3;");
            connection.Open();
        }
        private static void disconnect() {
            connection.Close();
        }

        public static void initDB() {
            createDatabase();
            addCharacterData();
        }

        public static void saveMatch(Game.Match match) {
            if (connection == null || connection.State != ConnectionState.Open) connect();
            string sql = string.Format(db_saveMatch + "VALUES({0}, '{1}', {2}, '{3}', {4})", match.Winner.number, match.player1.name, match.player1.character, match.player2.name, match.player2.character);
            SQLiteCommand command = new SQLiteCommand(sql, connection);
            command.ExecuteNonQuery();

            command = new SQLiteCommand(db_lastInsert, connection);
            SQLiteDataReader reader = command.ExecuteReader();
            int matchId = 0;
            while (reader.Read()) {
                matchId = int.Parse(reader["id"].ToString());
            }

            disconnect();
            if (connection == null || connection.State != ConnectionState.Open) connect();

            foreach (Game.Round r in match.rounds) {
                sql = string.Format(db_saveRound + "VALUES({0}, {1}, {2}, {3}, {4}, {5})", matchId, r.number, r.winner.number, r.timeLeft, r.player1HPLeft, r.player2HPLeft);
                command = new SQLiteCommand(sql, connection);
                command.ExecuteNonQuery();
            }

            disconnect();
        }

        private static void addCharacterData() {
            if (selectCharacters().Count == 0) {
                nonquery(db_characterData);
            }
        }

        private static void createDatabase() {
            string executableLocation = Path.GetDirectoryName(
                Assembly.GetExecutingAssembly().Location);
            string dbLocation = Path.Combine(executableLocation, dbFileName);
            Dictionary<string, string> neededTables = new Dictionary<string, string> { { "character", db_createCharacterTable }, { "match", db_createMatchTable }, { "round", db_createRoundTable } };
            if (!File.Exists(dbLocation)) {
                Console.WriteLine("DB file created");
                SQLiteConnection.CreateFile(dbFileName);

                foreach (var nt in neededTables) {
                    nonquery(nt.Value);
                }
            } else {
                Console.WriteLine("DB file already exists");

                List<string> tables = getTableList();
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
                        nonquery(nt.Value);
                    }
                }
                
            }
        }

        private static List<string[]> selectCharacters() {
            if (connection == null || connection.State != ConnectionState.Open) connect();

            SQLiteCommand command = new SQLiteCommand("select * from character", connection);
            SQLiteDataReader reader = command.ExecuteReader();

            List<string[]> res = new List<string[]>();
            while (reader.Read()) {
                Console.WriteLine(reader["id"].ToString() + ", " + (string)reader["name"] + ", " + (string)reader["name_short"]);
                res.Add(new string[] { reader["id"].ToString(), (string)reader["name"], (string)reader["name_short"] });
            }
            disconnect();

            return res;
        }

        private static List<string> getTableList() {
            if (connection == null || connection.State != ConnectionState.Open) connect();

            SQLiteCommand command = new SQLiteCommand(db_tableList, connection);
            SQLiteDataReader reader = command.ExecuteReader();

            List<string> tableList = new List<string>();
            while (reader.Read()) {
                tableList.Add((string)reader["name"]);
            }

            disconnect();
            return tableList;
        }

        private static void nonquery(string q) {
            if (connection == null || connection.State != ConnectionState.Open) connect();

            SQLiteCommand command = new SQLiteCommand(q, connection);
            command.ExecuteNonQuery();

            disconnect();
        }
    }
}
