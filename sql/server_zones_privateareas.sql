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
-- Table structure for table `server_zones_privateareas`
--

DROP TABLE IF EXISTS `server_zones_privateareas`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `server_zones_privateareas` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `parentZoneId` int(10) unsigned NOT NULL,
  `privateAreaName` varchar(32) NOT NULL,
  `className` varchar(32) NOT NULL,
  `dayMusic` smallint(6) unsigned DEFAULT '0',
  `nightMusic` smallint(6) unsigned DEFAULT '0',
  `battleMusic` smallint(6) unsigned DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `server_zones_privateareas`
--

LOCK TABLES `server_zones_privateareas` WRITE;
/*!40000 ALTER TABLE `server_zones_privateareas` DISABLE KEYS */;
INSERT INTO `server_zones_privateareas` VALUES (1,175,'PrivateAreaMasterPast','PrivateAreaMasterPast',0,0,0);
INSERT INTO `server_zones_privateareas` VALUES (2,230,'PrivateAreaMasterPast','PrivateAreaMasterPast',0,0,0);
INSERT INTO `server_zones_privateareas` VALUES (3,193,'ContentSimpleContent30002','PrivateAreaMasterSimpleContent',0,0,0);
/*!40000 ALTER TABLE `server_zones_privateareas` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2016-06-07 22:54:54
