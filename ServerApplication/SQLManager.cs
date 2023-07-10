using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;

namespace ServerApplication
{
    public class SQLManager
    {
        static MySqlConnection Connection;

        public SQLManager()
        {
            if (Connection == null) Connection = new MySqlConnection(SQLString.CONNECTION_STRING);
        }

        public int Login(string login)
        {
            Connection.Open();
            var com = Connection.CreateCommand();
            com.CommandText = $"SELECT * FROM Players WHERE PlayerName = '{login}'"; //Add additional check for IS PLAYER CONNECTED
            var reader = com.ExecuteReader();
            int count = 0;
            int id = -1;
            while (reader.Read())
            {
                id = reader.GetInt32("PlayerID");
                count++;
            }
            Connection.Close();
            if (count == 1) return id; //MAKE PLAYER CONNECTED
            else return -1;
        }
        public int ConnectToGame(int id)
        {
            Connection.Open();
            var com = Connection.CreateCommand();
            com.CommandText = $"SELECT * FROM waitlist";
            var reader = com.ExecuteReader();
            if (reader.Read())
            {
                int dbID = reader.GetInt32("PlayerID");
                int GameID = reader.GetInt32("GameID");
                if (dbID == id)
                {
                    Connection.Close();
                    return GameID;
                }
                Connection.Close();
                RemovePlayerFromWaitList(dbID);
                return CreateGame(dbID, id, GameID);
            }
            Connection.Close();
            return AddPlayerToWaitList(id);
        }
        private int AddPlayerToWaitList(int id)
        {
            int GameId = CalculateGameID();
            Connection.Open();
            var com = Connection.CreateCommand();
            com.CommandText = $"INSERT INTO waitlist (PlayerID,GameID) VALUES ({id},{GameId})";
            com.ExecuteNonQuery();
            Connection.Close();
            return GameId;
        }
        private void RemovePlayerFromWaitList(int id)
        {
            Connection.Open();
            var com = Connection.CreateCommand();
            com.CommandText = $"DELETE FROM `triviagame`.`waitlist` WHERE(`PlayerID` = '{id}')";
            com.ExecuteNonQuery();
            Connection.Close();

        }
        public int CreateGame(int firstPlayer, int secondPlayer,int gameID)
        {
            var questionIDs = GenerateFiveQuestions();
            Connection.Open();
            var com = Connection.CreateCommand();
            com.CommandText = $"INSERT INTO currentgames (`GameID`, `FirstPlayerID`, " +
                $"`FirstPlayerScore`, `SecondPlayerID`, `SecondPlayerScore`, `FirstQuestionID`, " +
                $"`SecondQuestionID`, `ThirdQuestionID`, `ForthQuestionID`, `FifthQuestionID`, " +
                $"`CurrentQuestionNumber`) " +
                $"VALUES ('{gameID}', '{firstPlayer}', '0', '{secondPlayer}', '0', '{questionIDs[0]}', '{questionIDs[1]}', '{questionIDs[2]}', '{questionIDs[3]}', '{questionIDs[4]}', '0');";
            com.ExecuteNonQuery();

            Connection.Close();
            return gameID;

        }
        private int CalculateGameID()
        {
            Connection.Open();
            var com = Connection.CreateCommand();
            com.CommandText = "SELECT MAX(GameID) FROM Games;";
            var read = com.ExecuteReader();
            int maxFromGames;
            try
            {
                read.Read();
                maxFromGames = int.Parse(read.GetString(0));
            }
            catch { maxFromGames = 0; }
            Connection.Close();

            Connection.Open();
            com = Connection.CreateCommand();
            com.CommandText = "SELECT MAX(GameID) FROM Games;";
            read = com.ExecuteReader();
            int maxFromCurrentGames;
            try
            {
                read.Read();
                maxFromCurrentGames = int.Parse(read.GetString(0));
            }
            catch { maxFromCurrentGames = 0; }
            Connection.Close();
            maxFromGames = Math.Max(maxFromGames, maxFromCurrentGames);
            return maxFromGames+1;
        }
        private int[] GenerateFiveQuestions()
        {
            return new int[] { 1, 2, 3, 4, 5 };
        }

        public bool CheckIfGameIsReady(int gameId)
        {
            Connection.Open();

            var com = Connection.CreateCommand();
            com.CommandText = $"SELECT COUNT(GameID) FROM currentgames WHERE GameID = {gameId}";
            var read = com.ExecuteReader();
            read.Read();
            
            if (read.GetInt32(0) !=0)
            {
                Connection.Close();
                return true;
            }

            Connection.Close();
            return false;
        }
    }
}