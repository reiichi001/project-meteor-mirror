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
-- Table structure for table `server_battlenpc_pools`
--

DROP TABLE IF EXISTS `server_battlenpc_pools`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `server_battlenpc_pools` (
  `poolId` int(10) unsigned NOT NULL,
  `actorClassId` int(10) unsigned NOT NULL,
  `name` varchar(50) NOT NULL,
  `genusId` int(10) unsigned NOT NULL,
  `currentJob` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `combatSkill` tinyint(3) unsigned NOT NULL,
  `combatDelay` smallint(5) unsigned NOT NULL,
  `combatDmgMult` float unsigned NOT NULL DEFAULT '1',
  `aggroType` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `immunity` int(10) unsigned NOT NULL DEFAULT '0',
  `linkType` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `spellListId` int(10) unsigned NOT NULL DEFAULT '0',
  `skillListId` int(10) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`poolId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `server_battlenpc_pools`
--

LOCK TABLES `server_battlenpc_pools` WRITE;
/*!40000 ALTER TABLE `server_battlenpc_pools` DISABLE KEYS */;
set autocommit=0;
INSERT INTO `server_battlenpc_pools` VALUES (1,2104001,'wharf_rat',12,0,1,4200,1,0,0,0,0,0);
INSERT INTO `server_battlenpc_pools` VALUES (2,2201407,'bloodthirsty_wolf',3,0,1,4200,1,0,0,0,0,0);
INSERT INTO `server_battlenpc_pools` VALUES (3,2290005,'yda',29,2,1,4200,1,0,0,0,0,0);
INSERT INTO `server_battlenpc_pools` VALUES (4,2290006,'papalymo',29,22,1,4200,1,0,0,0,0,0);
/*!40000 ALTER TABLE `server_battlenpc_pools` ENABLE KEYS */;
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

-- Dump completed on 2017-10-11 14:47:40
