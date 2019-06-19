-- MySQL dump 10.13  Distrib 5.7.10, for Win64 (x86_64)
--
-- Host: localhost    Database: ffxiv_database
-- ------------------------------------------------------
-- Server version	5.7.10-log

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `characters_timers`
--

DROP TABLE IF EXISTS `characters_timers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `characters_timers` (
  `characterId` int(10) unsigned NOT NULL DEFAULT '0',
  `thousandmaws` int(10) unsigned DEFAULT '0',
  `dzemaeldarkhold` int(10) unsigned DEFAULT '0',
  `bowlofembers_hard` int(10) unsigned DEFAULT '0',
  `bowlofembers` int(10) unsigned DEFAULT '0',
  `thornmarch` int(10) unsigned DEFAULT '0',
  `aurumvale` int(10) unsigned DEFAULT '0',
  `cutterscry` int(10) unsigned DEFAULT '0',
  `battle_aleport` int(10) unsigned DEFAULT '0',
  `battle_hyrstmill` int(10) unsigned DEFAULT '0',
  `battle_goldenbazaar` int(10) unsigned DEFAULT '0',
  `howlingeye_hard` int(10) unsigned DEFAULT '0',
  `howlingeye` int(10) unsigned DEFAULT '0',
  `castrumnovum` int(10) unsigned DEFAULT '0',
  `bowlofembers_extreme` int(10) unsigned DEFAULT '0',
  `rivenroad` int(10) unsigned DEFAULT '0',
  `rivenroad_hard` int(10) unsigned DEFAULT '0',
  `behests` int(10) unsigned DEFAULT '0',
  `companybehests` int(10) unsigned DEFAULT '0',
  `returntimer` int(10) unsigned DEFAULT '0',
  `skirmish` int(10) unsigned DEFAULT '0',
  PRIMARY KEY (`characterId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `characters_timers`
--

LOCK TABLES `characters_timers` WRITE;
/*!40000 ALTER TABLE `characters_timers` DISABLE KEYS */;
/*!40000 ALTER TABLE `characters_timers` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2016-06-07 22:54:49
