﻿using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Common;
using Org.BouncyCastle.Crypto.Macs;

namespace ServerApplication
{
    public class SQLManager
    {
        string con_string = "Server=localhost; database=triviagame; UID=root; password=12345";
        MySqlConnection Connection;

        private void Connect()
        {
            Connection = new MySqlConnection(con_string);
            try
            {
                Connection.Open();
            }
            catch
            {
                Connection.Close();
            }
        }
        public SQLManager()
        {
            if (Connection == null) Connection = new MySqlConnection(con_string);
        }

        public int Login(string login)
        {
            Connect();
            var com = Connection.CreateCommand();
            com.CommandText = $"SELECT * FROM Players WHERE PlayerName = '{login}'";
            var reader = com.ExecuteReader();
            int id = -1;
            if (reader.Read())
            {
                id = reader.GetInt32("PlayerID");
                bool isCon = reader.GetBoolean("IsConnected");
                Connection.Close();
                if (isCon)
                {
                    return -2; //Player is Connected
                }
                Connect();
                com = Connection.CreateCommand();
                com.CommandText = $"UPDATE players SET IsConnected = 1 WHERE (PlayerID = {id});";
                com.ExecuteNonQuery();
                Connection.Close();
                return id; //Player ID

            }
            else
            {
                Connection.Close();
                return -1; //Player does not exist
            }
        }
        public int Register(string username)
        {

            var com = Connection.CreateCommand();
            Connect();
            com = Connection.CreateCommand();
            com.CommandText = $"INSERT INTO players (PlayerName) VALUES ('{username}');";
            com.ExecuteNonQuery();
            Connection.Close();
            return Login(username);
        }
        public int ConnectToGame(int id)
        {
            Connect();
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
        public GameData GetGameResults(int GameID)
        {
            int[] res = new int[2];
            Connect();
            var com = Connection.CreateCommand();
            com.CommandText = $"SELECT * FROM games WHERE GameID = {GameID}";
            var reader = com.ExecuteReader();
            if (reader.Read())
            {
                var game = new GameData(reader);
                Connection.Close();
                return game;
            }
            else
            {
                var game = new GameData();
                game.WinnerID = 0;
                game.LoserID = 0;
                Connection.Close();
                return game;
            }
        }
        public bool CheckIfGameIsReady(int gameId)
        {
            Connect();

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
            Connect();

            var com = Connection.CreateCommand();
            com.CommandText = $"SELECT (SELECT PlayerName FROM Players WHERE PlayerID={userId}) as PlayerName," +
                $"(SELECT Count(WinnerPlayerID) FROM games Where WinnerPlayerId={userId} ) as Wons," +
                $"(SELECT Count(LoserPlayerID) FROM games Where LoserPlayerID={userId}) as Loses," +
                $"(SELECT Round(Avg(Wons+Loses)) From games) as Games;";
            var read = com.ExecuteReader();
            PlayerData playerData = new PlayerData(read);
            Connection.Close();
            return playerData;

        }
        public Question GetQuestion(int GameID)
        {
            Connect();

            var com = Connection.CreateCommand();
            com.CommandText = $"SELECT * FROM questions WHERE QuestionID = (SELECT IF( CurrentQuestionNumber = 0,FirstQuestionID,IF(CurrentQuestionNumber = 1,SecondQuestionID,IF(CurrentQuestionNumber = 2,ThirdQuestionID,IF(CurrentQuestionNumber = 3,ForthQuestionID,IF(CurrentQuestionNumber = 4,FifthQuestionID,0)))))FROM currentgames WHERE GameID = '{GameID}');";
            Question question = new Question(com.ExecuteReader());
            Connection.Close();
            return question;
        }
        public List<GameData> GetGameHystory(int PlayerID)
        {
            List<GameData> data = new List<GameData>();
            Connect();
            var com = Connection.CreateCommand();
            com.CommandText = $"SELECT * FROM triviagame.games WHERE WinnerPlayerID={PlayerID} OR LoserPlayerID={PlayerID};";
            var reader = com.ExecuteReader();
            if (!reader.HasRows)
            {
                Connection.Close();
                return data;
            }

            while (reader.Read())
            {
                data.Add(new GameData(reader));
            }
            Connection.Close();
            return data;
        }
        public void UpdatePlayerAnswer(int GameID, int PlayerID, float AnswerTime, bool IsAnswerRight)
        {
            Connect();
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


            Connect();
            QUERY += "Question";
            com = Connection.CreateCommand();
            com.CommandText = $"UPDATE currentgames SET {QUERY}AnswerTime = {AnswerTime.ToString().Replace(",", ".")} , {QUERY}Answer = {IsAnswerRight} WHERE GameID = {GameID};";
            com.ExecuteNonQuery();
            Connection.Close();
            CheckIfRoundChanged(GameID, roundName);

        }
        public void Disconnect(int PlayerID)
        {
            Connect();
            var com = Connection.CreateCommand();
            com.CommandText = $"UPDATE players SET IsConnected = 0 WHERE (PlayerID = {PlayerID});";
            com.ExecuteNonQuery();
            Connection.Close();
        }

        private void CheckIfRoundChanged(int GameID, string RoundName)
        {
            Connect();
            var com = Connection.CreateCommand();
            com.CommandText = $"UPDATE currentgames SET CurrentQuestionNumber = currentgames.CurrentQuestionNumber +(SELECT (IF(FirstPlayer{RoundName}QuestionAnswerTime=0,False,True) and IF(SecondPlayer{RoundName}QuestionAnswerTime=0,False,True)) as Ended) Where GameID = {GameID};";
            com.ExecuteNonQuery();
            Connection.Close();
            //https://localhost:44339/api/UpdatePlayerAnswer?GameID=2&PlayerID=2&AnswerTime=4&IsAnswerRight=False
            if (RoundName == "Fifth")
            {
                Connect();
                com = Connection.CreateCommand();
                com.CommandText = $"SELECT CurrentQuestionNumber FROM currentgames WHERE GameId= {GameID};";
                var read = com.ExecuteReader();
                read.Read();
                int lol = read.GetInt32(0);
                Connection.Close();
                if (lol == 5)
                {
                    MoveGameToHistory(GameID);
                }
            }
        }
        private void MoveGameToHistory(int GameID)
        {
            int[] Players = CalculateWinner(GameID);
            DeleteCurrentGame(GameID);

            Connect();
            var com = Connection.CreateCommand();
            com.CommandText = $"INSERT INTO games (GameID, WinnerPlayerID, LoserPlayerID) VALUES ({GameID}, {Players[0]}, {Players[1]});";
            com.ExecuteNonQuery();
            Connection.Close();
        }
        private int[] CalculateWinner(int GameID)
        {
            int[] TwoPlayers = CalculateTwoPlayersScore(GameID);
            Connect();
            var com = Connection.CreateCommand();
            com.CommandText = $"SELECT FirstPlayerID,SecondPlayerID FROM currentgames WHERE GameID = {GameID}";
            var reader = com.ExecuteReader();
            reader.Read();
            if (TwoPlayers[0] > TwoPlayers[1])
            {
                TwoPlayers[0] = reader.GetInt32(0);
                TwoPlayers[1] = reader.GetInt32(1);
            }
            else
            {
                TwoPlayers[0] = reader.GetInt32(1);
                TwoPlayers[1] = reader.GetInt32(0);
            }
            Connection.Close();
            return TwoPlayers;
        }
        private int[] CalculateTwoPlayersScore(int GameID)
        {
            Connect();
            var com = Connection.CreateCommand();
            com.CommandText = $"SELECT FirstPlayerID,SecondPlayerID,FirstPlayerFirstQuestionAnswerTime,FirstPlayerFirstQuestionAnswer,FirstPlayerSecondQuestionAnswerTime,FirstPlayerSecondQuestionAnswer,FirstPlayerThirdQuestionAnswerTime,FirstPlayerThirdQuestionAnswer,FirstPlayerForthQuestionAnswerTime,FirstPlayerForthQuestionAnswer,FirstPlayerFifthQuestionAnswerTime,FirstPlayerFifthQuestionAnswer, SecondPlayerFirstQuestionAnswerTime,SecondPlayerFirstQuestionAnswer,SecondPlayerSecondQuestionAnswerTime,SecondPlayerSecondQuestionAnswer,SecondPlayerThirdQuestionAnswerTime,SecondPlayerThirdQuestionAnswer,SecondPlayerForthQuestionAnswerTime,SecondPlayerForthQuestionAnswer,SecondPlayerFifthQuestionAnswerTime,SecondPlayerFifthQuestionAnswer FROM currentgames WHERE GameID = {GameID}";
            var reader = com.ExecuteReader();
            int firstPlayerScore = 0;
            int secondPlayerScore = 0;

            reader.Read();
            var FirstPlayerArray = new int[][] {
                new int[] { reader.GetInt32(2), reader.GetInt16(3) },
                new int[] { reader.GetInt32(4), reader.GetInt16(5) },
                new int[] { reader.GetInt32(6), reader.GetInt16(7) },
                new int[] { reader.GetInt32(8), reader.GetInt16(9) },
                new int[] { reader.GetInt32(10), reader.GetInt16(11) } };
            var SecondPlayerArray = new int[][]
            {
                new int[] { reader.GetInt32(12), reader.GetInt16(13) },
                new int[] { reader.GetInt32(14), reader.GetInt16(15) },
                new int[] { reader.GetInt32(16), reader.GetInt16(17) },
                new int[] { reader.GetInt32(18), reader.GetInt16(19) },
                new int[] { reader.GetInt32(20), reader.GetInt16(21) } };
            for (int i = 0; i < 5; i++)
            {

                if (FirstPlayerArray[i][1] != 0 && SecondPlayerArray[i][1] != 0)
                {

                    if (FirstPlayerArray[i][0] < SecondPlayerArray[i][0])
                    {
                        secondPlayerScore += 1;
                        firstPlayerScore += 2;
                    }
                    else
                    {
                        secondPlayerScore += 2;
                        firstPlayerScore += 1;
                    }
                }
                else if (FirstPlayerArray[i][1] == 1) firstPlayerScore += 2;
                else if (SecondPlayerArray[i][1] == 1) secondPlayerScore += 2;

            }
            Connection.Close();
            return new int[2] { firstPlayerScore, secondPlayerScore };
        }

        private void DeleteCurrentGame(int GameID)
        {
            Connect();
            var com = Connection.CreateCommand();
            com.CommandText = $"DELETE FROM currentgames WHERE (GameID = {GameID});";
            com.ExecuteNonQuery();
            Connection.Close();
        }
        private void RemovePlayerFromWaitList(int id)
        {
            Connect();
            var com = Connection.CreateCommand();
            com.CommandText = $"DELETE FROM `triviagame`.`waitlist` WHERE(`PlayerID` = '{id}')";
            com.ExecuteNonQuery();
            Connection.Close();

        }
        private int CreateGame(int firstPlayer, int secondPlayer, int gameID)
        {
            var questionIDs = GenerateFiveQuestions();
            Connect();
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
            Connect();
            var com = Connection.CreateCommand();
            com.CommandText = $"INSERT INTO waitlist (PlayerID,GameID) VALUES ({id},{GameId})";
            com.ExecuteNonQuery();
            Connection.Close();
            return GameId;
        }
        private int CalculateGameID()
        {
            Connect();
            var com = Connection.CreateCommand();
            com.CommandText = "SELECT GameID FROM games UNION SELECT GameID FROM currentgames ORDER BY GameID DESC;";
            var read = com.ExecuteReader();
            int maxFromGames = 0;
            if (read.Read()) { maxFromGames = read.GetInt32(0); }
            Connection.Close();
            return maxFromGames + 1;
        }
        private int InGamePlayerID(int GameID, int PlayerID)
        {
            Connect();
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
            Connect();
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
            Connect();
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
    public struct GameData
    {
        public int WinnerID;
        public int LoserID;
        public GameData(MySqlDataReader reader)
        {
            WinnerID = reader.GetInt32("WinnerPlayerID");
            LoserID = reader.GetInt32("LoserPlayerID");
        }
    }
}