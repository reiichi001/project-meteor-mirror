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
-- Table structure for table `gamedata_items_graphics`
--

SET autocommit = 0;

DROP TABLE IF EXISTS `gamedata_items_graphics`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `gamedata_items_graphics` (
  `catalogID` int(10) unsigned NOT NULL,
  `weaponId` int(10) unsigned NOT NULL,
  `equipmentId` int(10) unsigned NOT NULL,
  `variantId` int(10) unsigned NOT NULL,
  `colorId` int(10) unsigned NOT NULL,
  PRIMARY KEY (`catalogID`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `gamedata_items_graphics`
--

LOCK TABLES `gamedata_items_graphics` WRITE;
/*!40000 ALTER TABLE `gamedata_items_graphics` DISABLE KEYS */;
INSERT INTO `gamedata_items_graphics` VALUES (4020001,58,1,0,0);
INSERT INTO `gamedata_items_graphics` VALUES (4030010,76,1,90,0);
INSERT INTO `gamedata_items_graphics` VALUES (4030016,76,1,60,0);
INSERT INTO `gamedata_items_graphics` VALUES (4030117,76,2,20,0);
INSERT INTO `gamedata_items_graphics` VALUES (4030303,76,4,10,0);
INSERT INTO `gamedata_items_graphics` VALUES (4030407,79,1,0,0);
INSERT INTO `gamedata_items_graphics` VALUES (4030408,77,1,60,0);
INSERT INTO `gamedata_items_graphics` VALUES (4030507,78,1,0,0);
INSERT INTO `gamedata_items_graphics` VALUES (4030601,76,10,10,0);
INSERT INTO `gamedata_items_graphics` VALUES (4030602,76,10,0,0);
INSERT INTO `gamedata_items_graphics` VALUES (4030603,80,1,0,0);
INSERT INTO `gamedata_items_graphics` VALUES (4030604,76,14,0,0);
INSERT INTO `gamedata_items_graphics` VALUES (4030605,76,15,0,0);
INSERT INTO `gamedata_items_graphics` VALUES (4030606,76,16,0,0);
INSERT INTO `gamedata_items_graphics` VALUES (4030607,76,13,0,0);
INSERT INTO `gamedata_items_graphics` VALUES (4030608,76,17,0,0);
INSERT INTO `gamedata_items_graphics` VALUES (4030711,76,5,20,0);
INSERT INTO `gamedata_items_graphics` VALUES (4040001,141,1,70,0);
INSERT INTO `gamedata_items_graphics` VALUES (4040013,141,6,0,0);
INSERT INTO `gamedata_items_graphics` VALUES (4040109,141,8,0,0);
INSERT INTO `gamedata_items_graphics` VALUES (4040501,141,7,0,0);
INSERT INTO `gamedata_items_graphics` VALUES (4040502,141,7,10,0);
INSERT INTO `gamedata_items_graphics` VALUES (4040504,141,10,0,0);
INSERT INTO `gamedata_items_graphics` VALUES (4040505,141,11,0,0);
INSERT INTO `gamedata_items_graphics` VALUES (4040506,141,12,0,0);
INSERT INTO `gamedata_items_graphics` VALUES (4040507,141,9,0,0);
INSERT INTO `gamedata_items_graphics` VALUES (4070001,201,1,60,0);
INSERT INTO `gamedata_items_graphics` VALUES (4080201,161,3,50,0);
INSERT INTO `gamedata_items_graphics` VALUES (4100206,31,3,11,0);
INSERT INTO `gamedata_items_graphics` VALUES (4100801,31,12,0,0);
INSERT INTO `gamedata_items_graphics` VALUES (4100802,31,13,0,0);
INSERT INTO `gamedata_items_graphics` VALUES (4100803,31,14,0,0);
INSERT INTO `gamedata_items_graphics` VALUES (5020001,281,1,100,4);
INSERT INTO `gamedata_items_graphics` VALUES (5030101,331,1,20,0);
INSERT INTO `gamedata_items_graphics` VALUES (8011001,0,23,5,1);
INSERT INTO `gamedata_items_graphics` VALUES (8011708,0,15,16,0);
INSERT INTO `gamedata_items_graphics` VALUES (8011709,0,15,12,0);
INSERT INTO `gamedata_items_graphics` VALUES (8030245,0,7,18,0);
INSERT INTO `gamedata_items_graphics` VALUES (8030445,0,4,11,0);
INSERT INTO `gamedata_items_graphics` VALUES (8030601,0,9,21,0);
INSERT INTO `gamedata_items_graphics` VALUES (8030701,0,10,13,0);
INSERT INTO `gamedata_items_graphics` VALUES (8030801,0,13,19,0);
INSERT INTO `gamedata_items_graphics` VALUES (8031120,0,31,1,0);
INSERT INTO `gamedata_items_graphics` VALUES (8031716,0,15,16,0);
INSERT INTO `gamedata_items_graphics` VALUES (8031719,0,15,12,0);
INSERT INTO `gamedata_items_graphics` VALUES (8032834,0,59,1,0);
INSERT INTO `gamedata_items_graphics` VALUES (8040001,0,1,5,0);
INSERT INTO `gamedata_items_graphics` VALUES (8040002,0,1,5,2);
INSERT INTO `gamedata_items_graphics` VALUES (8040003,0,1,5,3);
INSERT INTO `gamedata_items_graphics` VALUES (8040004,0,1,5,0);
INSERT INTO `gamedata_items_graphics` VALUES (8040005,0,1,0,0);
INSERT INTO `gamedata_items_graphics` VALUES (8040006,0,1,5,3);
INSERT INTO `gamedata_items_graphics` VALUES (8040007,0,1,15,1);
INSERT INTO `gamedata_items_graphics` VALUES (8040008,0,1,5,0);
INSERT INTO `gamedata_items_graphics` VALUES (8040009,0,1,5,1);
INSERT INTO `gamedata_items_graphics` VALUES (8040010,0,1,15,0);
INSERT INTO `gamedata_items_graphics` VALUES (8040011,0,1,15,1);
INSERT INTO `gamedata_items_graphics` VALUES (8040012,0,1,6,0);
INSERT INTO `gamedata_items_graphics` VALUES (8040013,0,1,5,2);
INSERT INTO `gamedata_items_graphics` VALUES (8040014,0,1,5,0);
INSERT INTO `gamedata_items_graphics` VALUES (8040015,0,1,5,2);
INSERT INTO `gamedata_items_graphics` VALUES (8050031,0,2,6,0);
INSERT INTO `gamedata_items_graphics` VALUES (8050245,0,4,11,0);
INSERT INTO `gamedata_items_graphics` VALUES (8050346,0,5,11,0);
INSERT INTO `gamedata_items_graphics` VALUES (8050621,0,9,25,0);
INSERT INTO `gamedata_items_graphics` VALUES (8050622,0,9,24,0);
INSERT INTO `gamedata_items_graphics` VALUES (8050728,0,10,10,0);
INSERT INTO `gamedata_items_graphics` VALUES (8050808,0,15,22,0);
INSERT INTO `gamedata_items_graphics` VALUES (8051015,0,7,1,0);
INSERT INTO `gamedata_items_graphics` VALUES (8060001,0,1,1,0);
INSERT INTO `gamedata_items_graphics` VALUES (8060002,0,1,2,0);
INSERT INTO `gamedata_items_graphics` VALUES (8060003,0,1,0,0);
INSERT INTO `gamedata_items_graphics` VALUES (8060004,0,1,1,0);
INSERT INTO `gamedata_items_graphics` VALUES (8060005,0,1,1,0);
INSERT INTO `gamedata_items_graphics` VALUES (8060006,0,1,0,0);
INSERT INTO `gamedata_items_graphics` VALUES (8060007,0,1,2,0);
INSERT INTO `gamedata_items_graphics` VALUES (8060008,0,1,1,0);
INSERT INTO `gamedata_items_graphics` VALUES (8060009,0,1,1,0);
INSERT INTO `gamedata_items_graphics` VALUES (8060010,0,1,0,0);
INSERT INTO `gamedata_items_graphics` VALUES (8060011,0,1,2,0);
INSERT INTO `gamedata_items_graphics` VALUES (8060012,0,1,2,0);
INSERT INTO `gamedata_items_graphics` VALUES (8060013,0,1,6,0);
INSERT INTO `gamedata_items_graphics` VALUES (8060014,0,1,1,0);
INSERT INTO `gamedata_items_graphics` VALUES (8060015,0,1,2,0);
INSERT INTO `gamedata_items_graphics` VALUES (8070243,0,11,7,5);
INSERT INTO `gamedata_items_graphics` VALUES (8070346,0,5,11,0);
INSERT INTO `gamedata_items_graphics` VALUES (8080246,0,4,10,0);
INSERT INTO `gamedata_items_graphics` VALUES (8080346,0,5,12,0);
INSERT INTO `gamedata_items_graphics` VALUES (8080501,0,10,13,0);
INSERT INTO `gamedata_items_graphics` VALUES (8080601,0,25,7,0);
INSERT INTO `gamedata_items_graphics` VALUES (8081208,0,15,16,0);
INSERT INTO `gamedata_items_graphics` VALUES (8081209,0,15,12,0);
INSERT INTO `gamedata_items_graphics` VALUES (8090208,0,4,0,0);
INSERT INTO `gamedata_items_graphics` VALUES (8090307,0,6,0,0);
/*!40000 ALTER TABLE `gamedata_items_graphics` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

COMMIT;

-- Dump completed on 2016-06-07 22:54:52
