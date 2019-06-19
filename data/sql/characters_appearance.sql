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
-- Table structure for table `characters_appearance`
--

DROP TABLE IF EXISTS `characters_appearance`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `characters_appearance` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `characterId` int(10) unsigned NOT NULL,
  `baseId` int(10) unsigned NOT NULL,
  `size` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `voice` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `skinColor` smallint(5) unsigned NOT NULL,
  `hairStyle` smallint(5) unsigned NOT NULL,
  `hairColor` smallint(5) unsigned NOT NULL,
  `hairHighlightColor` smallint(5) unsigned NOT NULL DEFAULT '0',
  `hairVariation` smallint(5) unsigned NOT NULL,
  `eyeColor` smallint(5) unsigned NOT NULL,
  `faceType` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `faceEyebrows` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `faceEyeShape` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `faceIrisSize` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `faceNose` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `faceMouth` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `faceFeatures` tinyint(3) unsigned NOT NULL,
  `ears` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `characteristics` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `characteristicsColor` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `mainhand` int(10) unsigned NOT NULL,
  `offhand` int(10) unsigned NOT NULL,
  `head` int(10) unsigned NOT NULL,
  `body` int(10) unsigned NOT NULL,
  `hands` int(10) unsigned NOT NULL,
  `legs` int(10) unsigned NOT NULL,
  `feet` int(10) unsigned NOT NULL,
  `waist` int(10) unsigned NOT NULL,
  `leftFinger` int(10) unsigned NOT NULL DEFAULT '0',
  `rightFinger` int(10) unsigned NOT NULL DEFAULT '0',
  `leftEar` int(10) unsigned NOT NULL DEFAULT '0',
  `rightEar` int(10) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `characters_appearance`
--

LOCK TABLES `characters_appearance` WRITE;
/*!40000 ALTER TABLE `characters_appearance` DISABLE KEYS */;

/*!40000 ALTER TABLE `characters_appearance` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2016-06-07 22:54:43
