-- MySQL dump 10.13  Distrib 5.7.18, for Win64 (x86_64)
--
-- Host: localhost    Database: ffxiv_server
-- ------------------------------------------------------
-- Server version	5.7.18-log

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
-- Table structure for table `server_battlenpc_spawn_locations`
--

DROP TABLE IF EXISTS `server_battlenpc_spawn_locations`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `server_battlenpc_spawn_locations` (
  `bnpcId` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `customDisplayName` varchar(32) NOT NULL DEFAULT '',
  `groupId` int(10) unsigned NOT NULL,
  `positionX` float NOT NULL,
  `positionY` float NOT NULL,
  `positionZ` float NOT NULL,
  `rotation` float NOT NULL,
  PRIMARY KEY (`bnpcId`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `server_battlenpc_spawn_locations`
--

LOCK TABLES `server_battlenpc_spawn_locations` WRITE;
/*!40000 ALTER TABLE `server_battlenpc_spawn_locations` DISABLE KEYS */;
set autocommit=0;
INSERT INTO `server_battlenpc_spawn_locations` VALUES (1,'test',1,25.584,200,-450,-2.514);
INSERT INTO `server_battlenpc_spawn_locations` VALUES (2,'test',1,20,200,-444,-3.14);
INSERT INTO `server_battlenpc_spawn_locations` VALUES (3,'bloodthirsty_wolf',2,-3.02,17.35,14.24,-2.81);
INSERT INTO `server_battlenpc_spawn_locations` VALUES (4,'bloodthirsty_wolf',2,0.02,17.35,14.24,-2.81);
INSERT INTO `server_battlenpc_spawn_locations` VALUES (5,'bloodthirsty_wolf',2,-6.02,17.35,14.24,-2.81);
INSERT INTO `server_battlenpc_spawn_locations` VALUES (6,'yshtola',3,-8,16.35,6,0.5);
INSERT INTO `server_battlenpc_spawn_locations` VALUES (7,'papalymo',4,0,16.35,22,3);
/*!40000 ALTER TABLE `server_battlenpc_spawn_locations` ENABLE KEYS */;
UNLOCK TABLES;
commit;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2017-09-16  2:43:52
