CREATE DATABASE  IF NOT EXISTS `triviagame` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `triviagame`;
-- MySQL dump 10.13  Distrib 8.0.33, for Win64 (x86_64)
--
-- Host: 127.0.0.1    Database: triviagame
-- ------------------------------------------------------
-- Server version	8.0.33

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `currentgames`
--

DROP TABLE IF EXISTS `currentgames`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `currentgames` (
  `GameID` int NOT NULL,
  `FirstPlayerID` int NOT NULL,
  `SecondPlayerID` int NOT NULL,
  `FirstQuestionID` int NOT NULL,
  `SecondQuestionID` int NOT NULL,
  `ThirdQuestionID` int NOT NULL,
  `ForthQuestionID` int NOT NULL,
  `FifthQuestionID` int NOT NULL,
  `CurrentQuestionNumber` int NOT NULL DEFAULT '0',
  `FirstPlayerFirstQuestionAnswerTime` float DEFAULT '0',
  `SecondPlayerFirstQuestionAnswerTime` float DEFAULT '0',
  `FirstPlayerFirstQuestionAnswer` tinyint DEFAULT '0',
  `SecondPlayerFirstQuestionAnswer` tinyint DEFAULT '0',
  `FirstPlayerSecondQuestionAnswerTime` float DEFAULT '0',
  `SecondPlayerSecondQuestionAnswerTime` float DEFAULT '0',
  `FirstPlayerSecondQuestionAnswer` tinyint DEFAULT '0',
  `SecondPlayerSecondQuestionAnswer` tinyint DEFAULT '0',
  `FirstPlayerThirdQuestionAnswerTime` float DEFAULT '0',
  `SecondPlayerThirdQuestionAnswerTime` float DEFAULT '0',
  `FirstPlayerThirdQuestionAnswer` tinyint DEFAULT '0',
  `SecondPlayerThirdQuestionAnswer` tinyint DEFAULT '0',
  `FirstPlayerForthQuestionAnswerTime` float DEFAULT '0',
  `SecondPlayerForthQuestionAnswerTime` float DEFAULT '0',
  `FirstPlayerForthQuestionAnswer` tinyint DEFAULT '0',
  `SecondPlayerForthQuestionAnswer` tinyint DEFAULT '0',
  `FirstPlayerFifthQuestionAnswerTime` float DEFAULT '0',
  `SecondPlayerFifthQuestionAnswerTime` float DEFAULT '0',
  `FirstPlayerFifthQuestionAnswer` tinyint DEFAULT '0',
  `SecondPlayerFifthQuestionAnswer` tinyint DEFAULT '0',
  PRIMARY KEY (`GameID`),
  UNIQUE KEY `GameID_UNIQUE` (`GameID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `currentgames`
--

LOCK TABLES `currentgames` WRITE;
/*!40000 ALTER TABLE `currentgames` DISABLE KEYS */;
INSERT INTO `currentgames` VALUES (1,1,2,5,11,20,7,14,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0);
/*!40000 ALTER TABLE `currentgames` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2023-07-14 15:45:21
