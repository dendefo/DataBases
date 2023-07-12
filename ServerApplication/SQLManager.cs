using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.UI.WebControls;
using Microsoft.Ajax.Utilities;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Crypto.Prng;

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
        public int Register(string username)
        {

            var com = Connection.CreateCommand();
            Connection.Open();
            com = Connection.CreateCommand();
            com.CommandText = $"INSERT INTO players (PlayerName) VALUES ('{username}');";
            com.ExecuteNonQuery();
            Connection.Close();
            return Login(username);
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
        public int CreateGame(int firstPlayer, int secondPlayer, int gameID)
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
            return maxFromGames + 1;
        }
        private List<int> GenerateFiveQuestions()
        {
            Connection.Open();
            var com = Connection.CreateCommand();
            com.CommandText = "SELECT Count(QuestionID) FROM questions";
            var reader = com.ExecuteReader();
            Random rand = new Random();
            List<int> generatedQuestions = new List<int>();
            reader.Read();
            while (generatedQuestions.Count < 5)
            {
                var random = rand.Next(1, reader.GetInt32(0)+1);
                if (generatedQuestions.Contains(random))
                {
                    continue;
                }
                else
                generatedQuestions.Add(random);
            }
            Connection.Close();
            return generatedQuestions;
        }

        public bool CheckIfGameIsReady(int gameId)
        {
            Connection.Open();

            var com = Connection.CreateCommand();
            com.CommandText = $"SELECT COUNT(GameID) FROM currentgames WHERE GameID = {gameId}";
            var read = com.ExecuteReader();
            read.Read();

            if (read.GetInt32(0) != 0)
            {
                Connection.Close();
                return true;
            }

            Connection.Close();
            return false;
        }
        public string GetUsername(int userId)
        {
            Connection.Open();

            var com = Connection.CreateCommand();
            com.CommandText = $"SELECT PlayerName FROM players WHERE PlayerID = {userId}";
            var read = com.ExecuteReader();
            read.Read();
            string username = read.GetString("PlayerName");
            Connection.Close();
            return username;

        }

        public Question GetQuestion(int questionId)
        {
            Connection.Open();

            var com = Connection.CreateCommand();
            com.CommandText = $"SELECT * FROM questions WHERE QuestionID = (SELECT IF( CurrentQuestionNumber = 0,FirstQuestionID,IF(CurrentQuestionNumber = 1,SecondQuestionID,IF(CurrentQuestionNumber = 2,ThirdQuestionID,IF(CurrentQuestionNumber = 3,ForthQuestionID,FifthQuestionID))))FROM currentgames WHERE GameID = '1');";
            Question question = new Question(com.ExecuteReader());

            Connection.Close();
            return question;
        }
    }

    public struct Question
    {
        public int QuestionId;
        public string QuestionText;
        public string[] Answers;
        public int CorrectAnswerIndex;

        public Question(MySqlDataReader reader)
        {
            if (reader.Read())
            {
                QuestionId = reader.GetInt32(0);
                QuestionText = reader.GetString(1);
                CorrectAnswerIndex = reader.GetInt32(2);
                Answers = new string[4];
                for (int i = 0; i < 4; i++)
                {
                    Answers[i] = reader.GetString(i + 3);
                }
            }
            else
            {
                QuestionId = 0;
                QuestionText = "";
                CorrectAnswerIndex = 0;
                Answers = new string[0];
            }
        }
    }
}