using System;
using System.Collections.Generic;
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
        public PlayerData GetPlayerData(int userId)
        {
            Connection.Open();

            var com = Connection.CreateCommand();
            com.CommandText = $"SELECT (SELECT PlayerName FROM Players WHERE PlayerID={userId}) as PlayerName,\r\n(SELECT Count(WinnerPlayerID) FROM games Where WinnerPlayerId={userId} ) as Wons,\r\n(SELECT Count(LoserPlayerID) FROM games Where LoserPlayerID={userId}) as Loses,\r\n(SELECT Round(Avg(Wons+Loses)) From games) as Games;";
            var read = com.ExecuteReader();
            PlayerData playerData = new PlayerData(read);
            Connection.Close();
            return playerData;

        }
        public Question GetQuestion(int GameID)
        {
            Connection.Open();

            var com = Connection.CreateCommand();
            com.CommandText = $"SELECT * FROM questions WHERE QuestionID = (SELECT IF( CurrentQuestionNumber = 0,FirstQuestionID,IF(CurrentQuestionNumber = 1,SecondQuestionID,IF(CurrentQuestionNumber = 2,ThirdQuestionID,IF(CurrentQuestionNumber = 3,ForthQuestionID,IF(CurrentQuestionNumber = 4,FifthQuestionID,0)))))FROM currentgames WHERE GameID = '{GameID}');";
            Question question = new Question(com.ExecuteReader());

            Connection.Close();
            return question;
        }
        public void UpdatePlayerAnswer(int GameID, int PlayerID, float AnswerTime, bool IsAnswerRight)
        {
            Connection.Open();
            var com = Connection.CreateCommand();
            com.CommandText = "SELECT GameID FROM currentgames;";
            var reader = com.ExecuteReader();
            var list = new List<int>();
            while (reader.Read())
            {
                list.Add(reader.GetInt32(0));
            }
            Connection.Close();
            if (!list.Contains(GameID)) return;
            int AnswerID = GetInGameQuestionID(GameID);
            int inGamePlayerID = InGamePlayerID(GameID, PlayerID);
            string QUERY = "";

            if (inGamePlayerID == 0) QUERY = "FirstPlayer";
            else QUERY = "SecondPlayer";

            string roundName = CalculateRoundName(AnswerID);
            QUERY += roundName;


            Connection.Open();
            QUERY += "Question";
            com = Connection.CreateCommand();
            com.CommandText = $"UPDATE currentgames SET {QUERY}AnswerTime = {AnswerTime.ToString().Replace(",", ".")} , {QUERY}Answer = {IsAnswerRight} WHERE GameID = {GameID};";
            com.ExecuteNonQuery();
            Connection.Close();
            CheckIfRoundChanged(GameID, roundName);

        }

        private void CheckIfRoundChanged(int GameID, string RoundName)
        {
            Connection.Open();
            var com = Connection.CreateCommand();
            com.CommandText = $"UPDATE currentgames SET CurrentQuestionNumber = currentgames.CurrentQuestionNumber +(SELECT (IF(FirstPlayer{RoundName}QuestionAnswerTime=0,False,True) and IF(SecondPlayer{RoundName}QuestionAnswerTime=0,False,True)) as Ended) Where GameID = {GameID};";

            if (com.ExecuteNonQuery() != 0 && RoundName == "Fifth")
            {
                Connection.Close();
                MoveGameToHistory(GameID);
            }
            else
            {
                Connection.Close();
            }
        }
        private void MoveGameToHistory(int GameID)
        {
            int[] Players = CalculateWinner(GameID);
            DeleteCurrentGame(GameID);
            //Add to Games

        }
        private int[] CalculateWinner(int GameID)
        {
            return new int[2] { 0, 1 }; //First ID is Winner, second id is Loser
        }
        private void DeleteCurrentGame(int GameID)
        {
            Connection.Open();
            var com = Connection.CreateCommand();
            com.CommandText = $"DELETE FROM currentgames WHERE (GameID = {GameID});";
            com.ExecuteNonQuery();
            Connection.Close();
        }
        private void RemovePlayerFromWaitList(int id)
        {
            Connection.Open();
            var com = Connection.CreateCommand();
            com.CommandText = $"DELETE FROM `triviagame`.`waitlist` WHERE(`PlayerID` = '{id}')";
            com.ExecuteNonQuery();
            Connection.Close();

        }
        private int CreateGame(int firstPlayer, int secondPlayer, int gameID)
        {
            var questionIDs = GenerateFiveQuestions();
            Connection.Open();
            var com = Connection.CreateCommand();
            com.CommandText = $"INSERT INTO currentgames (`GameID`, `FirstPlayerID`, " +
                $"`SecondPlayerID`, `FirstQuestionID`,`SecondQuestionID`, `ThirdQuestionID`, " +
                $"`ForthQuestionID`, `FifthQuestionID`, `CurrentQuestionNumber`) " +
                $"VALUES ('{gameID}', '{firstPlayer}', '{secondPlayer}', '{questionIDs[0]}', '{questionIDs[1]}', '{questionIDs[2]}', '{questionIDs[3]}', '{questionIDs[4]}', '0');";
            com.ExecuteNonQuery();

            Connection.Close();
            return gameID;

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
        private int CalculateGameID()
        {
            Connection.Open();
            var com = Connection.CreateCommand();
            com.CommandText = "SELECT GameID FROM games UNION SELECT GameID FROM currentgames ORDER BY GameID DESC;";
            var read = com.ExecuteReader();
            read.Read();
            var maxFromGames = read.GetInt32(0);
            Connection.Close();
            return maxFromGames + 1;
        }
        private int InGamePlayerID(int GameID, int PlayerID)
        {
            Connection.Open();
            var com = Connection.CreateCommand();
            com.CommandText = $"SELECT FirstPlayerID FROM currentgames WHERE GameID={GameID}";
            var read = com.ExecuteReader();
            read.Read();
            var CurrentGamePlayerID = read.GetInt32("FirstPlayerID");
            Connection.Close();
            if (CurrentGamePlayerID == PlayerID)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }
        private int GetInGameQuestionID(int GameID)
        {
            Connection.Open();
            var com = Connection.CreateCommand();
            com.CommandText = $"SELECT CurrentQuestionNumber FROM currentgames Where GameID = {GameID};";
            var reader = com.ExecuteReader();
            reader.Read();
            var toReturn = reader.GetInt32(0);
            Connection.Close();
            return toReturn;
        }
        private string CalculateRoundName(int roundNumber)
        {
            switch (roundNumber)
            {
                case 0:
                    return "First";
                case 1:
                    return "Second";
                case 2:
                    return "Third";
                case 3:
                    return "Forth";
                case 4:
                    return "Fifth";
                default: return "";
            }
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
                var random = rand.Next(1, reader.GetInt32(0) + 1);
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
    public struct PlayerData
    {
        public string _name;
        public int _amountOfGames;
        public int _wins;
        public int _losses;
        public PlayerData(MySqlDataReader reader)
        {
            if (reader.Read())
            {
                _name = reader.GetString("PlayerName");
                _amountOfGames = reader.GetInt32("Games");
                _wins = reader.GetInt32("Wons");
                _losses = reader.GetInt32("Loses");
            }
            else
            {
                _name = null;
                _amountOfGames = 0;
                _wins = 0;
                _losses = 0;
            }
        }
    }
}