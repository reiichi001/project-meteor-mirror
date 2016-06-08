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
-- Table structure for table `server_zones_spawnlocations`
--

DROP TABLE IF EXISTS `server_zones_spawnlocations`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `server_zones_spawnlocations` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `zoneId` int(10) unsigned NOT NULL,
  `privateAreaName` varchar(32) DEFAULT NULL,
  `spawnType` tinyint(3) unsigned DEFAULT '0',
  `spawnX` float NOT NULL,
  `spawnY` float NOT NULL,
  `spawnZ` float NOT NULL,
  `spawnRotation` float NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `server_zones_spawnlocations`
--

LOCK TABLES `server_zones_spawnlocations` WRITE;
/*!40000 ALTER TABLE `server_zones_spawnlocations` DISABLE KEYS */;
INSERT INTO `server_zones_spawnlocations` VALUES (1,155,NULL,2,58.92,4,-1219.07,0.52);
INSERT INTO `server_zones_spawnlocations` VALUES (2,133,NULL,2,-444.266,39.518,191,1.9);
INSERT INTO `server_zones_spawnlocations` VALUES (3,175,NULL,2,-110.157,202,171.345,0);
INSERT INTO `server_zones_spawnlocations` VALUES (4,193,NULL,2,0.016,10.35,-36.91,0.025);
INSERT INTO `server_zones_spawnlocations` VALUES (5,166,NULL,2,356.09,3.74,-701.62,-1.4);
INSERT INTO `server_zones_spawnlocations` VALUES (6,175,'PrivateAreaMasterPast',2,12.63,196.05,131.01,-1.34);
INSERT INTO `server_zones_spawnlocations` VALUES (7,128,NULL,2,-8.48,45.36,139.5,2.02);
INSERT INTO `server_zones_spawnlocations` VALUES (8,230,'PrivateAreaMasterPast',0,-838.1,6,231.94,1.1);
INSERT INTO `server_zones_spawnlocations` VALUES (9,193,NULL,16,-5,16.35,6,0.5);
INSERT INTO `server_zones_spawnlocations` VALUES (10,166,NULL,16,356.09,3.74,-701.62,-1.4);
/*!40000 ALTER TABLE `server_zones_spawnlocations` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2016-06-07 22:54:55
