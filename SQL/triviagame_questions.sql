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
-- Table structure for table `questions`
--

DROP TABLE IF EXISTS `questions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `questions` (
  `QuestionId` int NOT NULL AUTO_INCREMENT,
  `QuestionText` varchar(150) NOT NULL,
  `RightAnswerNumber` int NOT NULL,
  `FirstAnswer` varchar(60) NOT NULL,
  `SecondAnswer` varchar(60) NOT NULL,
  `ThirdAnswer` varchar(60) NOT NULL,
  `ForthAnswer` varchar(60) NOT NULL,
  PRIMARY KEY (`QuestionId`),
  UNIQUE KEY `QuestionId_UNIQUE` (`QuestionId`)
) ENGINE=InnoDB AUTO_INCREMENT=22 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `questions`
--

LOCK TABLES `questions` WRITE;
/*!40000 ALTER TABLE `questions` DISABLE KEYS */;
INSERT INTO `questions` VALUES (1,'What is Love?',1,'Baby don\'t hurt me','I don\'t know:(','Chemical Reaction','Check \"Love is...\" gum'),(2,'In the game \"The Legend of Zelda: Ocarina of Time,\" what is the name of the horse Link rides?',4,'Epona','Zelda','Ganondorf','Saria'),(3,'Which video game series features the character Master Chief?',2,'Call of Duty','Halo','Battlefield','Gears of War'),(4,'Who is the main protagonist in the \"Metal Gear Solid\" series?',1,'Solid Snake','Liquid Snake','Big Boss','Revolver Ocelot'),(5,'Which game introduced the character Samus Aran?',1,'Metroid','Donkey Kong','Super Mario Bros.','The Legend of Zelda'),(6,'In the game \"BioShock,\" what is the name of the underwater city where the game takes place?',1,'Rapture','Columbia','Arkham City','Silent Hill'),(7,'Which game features the character Geralt of Rivia?',3,'Dark Souls','Dragon Age: Inquisition','The Witcher 3: Wild Hunt','Skyrim'),(8,'What is the name of the main character in the \"Final Fantasy VII\" game?',3,'Sephiroth','Tidus','Cloud Strife','Squall Leonhart'),(9,'In the game \"Red Dead Redemption 2,\" what is the name of the gang led by Dutch van der Linde?',4,'O\'Driscoll Gang','Morgan\'s Raiders','Marston\'s Gang','Van der Linde Gang'),(10,'Which game series features the character Nathan Drake?',2,'Assassin\'s Creed','Uncharted','Tomb Raider','God of War'),(11,'What is the name of the main character in the game \"Half-Life\"?',4,'Adrian Shephard','Chell','Alyx Vance','Gordon Freeman'),(12,'In the game \"Dark Souls,\" what is the name of the first area players explore?',2,'Firelink Shrine','Undead Burg','Anor Londo','Blighttown'),(13,'Which game introduced the character Kratos?',1,'God of Wa','Devil May Cry','Mortal Kombat','Resident Evil'),(14,'What is the name of the protagonist in the game \"The Last of Us\"?',3,'Ellie','Tommy','Joel','Tess'),(15,'In the game \"Fallout 4,\" which organization is responsible for rebuilding and governing post-apocalyptic Boston?',4,'Brotherhood of Steel','Institute','Minutemen','Railroad'),(16,'Which game features the character Lara Croft?',3,'Uncharted','Assassin\'s Creed','Tomb Raider','Splinter Cell'),(17,'In the game \"Overwatch,\" what role does the character Tracer primarily fulfill?',3,'Damage','Tank','Support','Flex'),(18,'What is the name of the virtual city in the game \"Grand Theft Auto V\"?',4,'Los Angeles','Vice City','San Andreas','Los Santos'),(19,'In the game \"The Elder Scrolls V: Skyrim,\" what is the name of the group of assassins that the player can join?',2,'Thieves Guild','Dark Brotherhood','Companions','Blades'),(20,'Which game introduced the character Solid Snake?',1,'Metal Gear','Splinter Cell','Resident Evil','Silent Hill'),(21,'In the game \"Mass Effect,\" what is the name of the main spaceship that the player commands?',2,'Sovereign','Normandy','Citadel','Tempest');
/*!40000 ALTER TABLE `questions` ENABLE KEYS */;
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
