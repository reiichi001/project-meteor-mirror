-- MySQL dump 10.13  Distrib 5.7.11, for Win64 (x86_64)
--
-- Host: localhost    Database: ffxiv_server
-- ------------------------------------------------------
-- Server version	5.7.11

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
-- Table structure for table `server_statuseffects`
--

DROP TABLE IF EXISTS `server_statuseffects`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `server_statuseffects` (
  `id` int(10) unsigned NOT NULL,
  `name` varchar(128) NOT NULL,
  `flags` int(10) unsigned NOT NULL DEFAULT '10',
  `overwrite` tinyint(3) unsigned NOT NULL DEFAULT '1',
  `tickMs` int(10) unsigned NOT NULL DEFAULT '3000',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `server_statuseffects`
--

LOCK TABLES `server_statuseffects` WRITE;
/*!40000 ALTER TABLE `server_statuseffects` DISABLE KEYS */;
set autocommit=0;
INSERT INTO `server_statuseffects` VALUES (223001,'quick',10,2,0);
INSERT INTO `server_statuseffects` VALUES (223002,'haste',18,2,0);
INSERT INTO `server_statuseffects` VALUES (223004,'petrification',16515074,2,0);
INSERT INTO `server_statuseffects` VALUES (223005,'paralysis',3932162,2,3000);
INSERT INTO `server_statuseffects` VALUES (223006,'silence',262154,2,0);
INSERT INTO `server_statuseffects` VALUES (223007,'blind',10,2,0);
INSERT INTO `server_statuseffects` VALUES (223008,'mute',262154,2,0);
INSERT INTO `server_statuseffects` VALUES (223010,'glare',10,2,0);
INSERT INTO `server_statuseffects` VALUES (223011,'poison',10,2,3000);
INSERT INTO `server_statuseffects` VALUES (223012,'transfixion',8388610,2,0);
INSERT INTO `server_statuseffects` VALUES (223013,'pacification',2621450,2,0);
INSERT INTO `server_statuseffects` VALUES (223014,'amnesia',1048586,2,0);
INSERT INTO `server_statuseffects` VALUES (223015,'stun',16515082,2,0);
INSERT INTO `server_statuseffects` VALUES (223016,'daze',16515082,2,0);
INSERT INTO `server_statuseffects` VALUES (223029,'hp_boost',18,2,0);
INSERT INTO `server_statuseffects` VALUES (223030,'hp_penalty',10,2,0);
INSERT INTO `server_statuseffects` VALUES (223058,'aegis_boon',65554,2,3000);
INSERT INTO `server_statuseffects` VALUES (223060,'outmaneuver',65554,2,3000);
INSERT INTO `server_statuseffects` VALUES (223062,'sentinel',18,2,3000);
INSERT INTO `server_statuseffects` VALUES (223068,'tempered_will',18,2,3000);
INSERT INTO `server_statuseffects` VALUES (223075,'featherfoot',1042,2,0);
INSERT INTO `server_statuseffects` VALUES (223094,'invigorate',18,2,2000);
INSERT INTO `server_statuseffects` VALUES (223129,'protect',10,2,0);
INSERT INTO `server_statuseffects` VALUES (223205,'combo',10,2,3000);
INSERT INTO `server_statuseffects` VALUES (223206,'goring_blade',10,2,3000);
INSERT INTO `server_statuseffects` VALUES (223209,'fists_of_fire',33554466,2,0);
INSERT INTO `server_statuseffects` VALUES (223210,'fists_of_earth',33554466,2,0);
INSERT INTO `server_statuseffects` VALUES (223211,'fists_of_wind',33554466,2,0);
INSERT INTO `server_statuseffects` VALUES (223245,'spinning_heel',18,1,0);
INSERT INTO `server_statuseffects` VALUES (253003,'evade_proc',15,1,0);
INSERT INTO `server_statuseffects` VALUES (253004,'block_proc',15,1,0);
INSERT INTO `server_statuseffects` VALUES (253005,'parry_proc',15,1,0);
INSERT INTO `server_statuseffects` VALUES (253006,'miss_proc',15,1,0);
/*!40000 ALTER TABLE `server_statuseffects` ENABLE KEYS */;
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

-- Dump completed on 2018-02-15  0:04:51
