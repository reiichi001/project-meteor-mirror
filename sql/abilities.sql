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
-- Table structure for table `abilities`
--

DROP TABLE IF EXISTS `abilities`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `abilities` (
  `id` smallint(5) unsigned NOT NULL,
  `name` varchar(255) NOT NULL,
  `classJob` tinyint(3) unsigned NOT NULL,
  `lvl` tinyint(3) unsigned NOT NULL,
  `requirements` smallint(5) unsigned NOT NULL,
  `validTarget` tinyint(3) unsigned NOT NULL,
  `aoeTarget` tinyint(3) unsigned NOT NULL,
  `aoeType` tinyint(3) unsigned NOT NULL,
  `range` int(10) unsigned NOT NULL,
  `characterFind` tinyint(3) unsigned NOT NULL,
  `statusDuration` int(10) unsigned NOT NULL,
  `castTime` int(10) unsigned NOT NULL,
  `recastTime` int(10) unsigned NOT NULL,
  `mpCost` smallint(5) unsigned NOT NULL,
  `tpCost` smallint(5) unsigned NOT NULL,
  `animationType` tinyint(3) unsigned NOT NULL,
  `effectAnimation` smallint(5) unsigned NOT NULL,
  `modelAnimation` smallint(5) unsigned NOT NULL,
  `animationDuration` smallint(5) unsigned NOT NULL,
  `positionBonus` tinyint(3) unsigned NOT NULL,
  `procRequirement` tinyint(3) unsigned NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `abilities`
--

LOCK TABLES `abilities` WRITE;
/*!40000 ALTER TABLE `abilities` DISABLE KEYS */;
/*!40000 ALTER TABLE `abilities` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2016-06-07 22:54:44
