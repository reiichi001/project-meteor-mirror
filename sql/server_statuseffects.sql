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
INSERT INTO `server_statuseffects` VALUES (223001,'quick',50,2,0);
INSERT INTO `server_statuseffects` VALUES (223002,'haste',50,2,0);
INSERT INTO `server_statuseffects` VALUES (223004,'petrification',264241194,2,0);
INSERT INTO `server_statuseffects` VALUES (223005,'paralysis',42,2,3000);
INSERT INTO `server_statuseffects` VALUES (223006,'silence',4194346,2,0);
INSERT INTO `server_statuseffects` VALUES (223007,'blind',42,2,0);
INSERT INTO `server_statuseffects` VALUES (223008,'mute',4194346,2,0);
INSERT INTO `server_statuseffects` VALUES (223010,'glare',42,2,0);
INSERT INTO `server_statuseffects` VALUES (223011,'poison',42,2,0);
INSERT INTO `server_statuseffects` VALUES (223012,'transfixion',268435498,2,0);
INSERT INTO `server_statuseffects` VALUES (223013,'pacification',8388650,2,0);
INSERT INTO `server_statuseffects` VALUES (223014,'amnesia',16777258,2,0);
INSERT INTO `server_statuseffects` VALUES (223015,'stun',264241194,2,0);
INSERT INTO `server_statuseffects` VALUES (223016,'daze',264241194,2,0);
INSERT INTO `server_statuseffects` VALUES (223029,'hp_boost',50,2,0);
INSERT INTO `server_statuseffects` VALUES (223030,'hp_penalty',42,2,0);
INSERT INTO `server_statuseffects` VALUES (223038,'defense_down',42,2,0);
INSERT INTO `server_statuseffects` VALUES (223058,'aegis_boon',528434,2,0);
INSERT INTO `server_statuseffects` VALUES (223062,'sentinel',1048626,2,0);
INSERT INTO `server_statuseffects` VALUES (223063,'cover',16434,2,0);
INSERT INTO `server_statuseffects` VALUES (223064,'rampart',50,2,0);
INSERT INTO `server_statuseffects` VALUES (223068,'tempered_will',50,2,0);
INSERT INTO `server_statuseffects` VALUES (223075,'featherfoot',131122,2,0);
INSERT INTO `server_statuseffects` VALUES (223078,'enduring_march',50,2,0);
INSERT INTO `server_statuseffects` VALUES (223081,'bloodbath',1048626,2,0);
INSERT INTO `server_statuseffects` VALUES (223083,'foresight',262194,2,0);
INSERT INTO `server_statuseffects` VALUES (223091,'keen_flurry',50,2,0);
INSERT INTO `server_statuseffects` VALUES (223094,'invigorate',50,2,0);
INSERT INTO `server_statuseffects` VALUES (223097,'collusion',1048626,1,0);
INSERT INTO `server_statuseffects` VALUES (223104,'quelling_strike',1058,2,0);
INSERT INTO `server_statuseffects` VALUES (223106,'hawks_eye',50,2,0);
INSERT INTO `server_statuseffects` VALUES (223108,'decoy',4130,2,0);
INSERT INTO `server_statuseffects` VALUES (223127,'bloodletter',42,2,0);
INSERT INTO `server_statuseffects` VALUES (223133,'stoneskin',16402,1,3000);
INSERT INTO `server_statuseffects` VALUES (223173,'covered',42,2,0);
INSERT INTO `server_statuseffects` VALUES (223180,'regen',50,2,0);
INSERT INTO `server_statuseffects` VALUES (223205,'combo',42,2,0);
INSERT INTO `server_statuseffects` VALUES (223206,'goring_blade',42,2,0);
INSERT INTO `server_statuseffects` VALUES (223207,'berserk2',1074806818,1,3000);
INSERT INTO `server_statuseffects` VALUES (223208,'rampage2',1075855394,1,5000);
INSERT INTO `server_statuseffects` VALUES (223209,'fists_of_fire',1073742882,2,0);
INSERT INTO `server_statuseffects` VALUES (223210,'fists_of_earth',1073742882,2,0);
INSERT INTO `server_statuseffects` VALUES (223211,'fists_of_wind',1073742882,2,0);
INSERT INTO `server_statuseffects` VALUES (223212,'power_surge_I',1058,2,0);
INSERT INTO `server_statuseffects` VALUES (223213,'power_surge_II',1058,2,0);
INSERT INTO `server_statuseffects` VALUES (223214,'power_surge_III',1058,2,0);
INSERT INTO `server_statuseffects` VALUES (223215,'life_surge_I',1048626,2,0);
INSERT INTO `server_statuseffects` VALUES (223216,'life_surge_II',1048626,2,0);
INSERT INTO `server_statuseffects` VALUES (223217,'life_surge_III',1048626,2,0);
INSERT INTO `server_statuseffects` VALUES (223218,'dread_spike',16418,2,0);
INSERT INTO `server_statuseffects` VALUES (223221,'raging_strike2',1074855970,1,0);
INSERT INTO `server_statuseffects` VALUES (223227,'cleric_stance',8226,1,0);
INSERT INTO `server_statuseffects` VALUES (223228,'blissful_mind',1073741858,1,1000);
INSERT INTO `server_statuseffects` VALUES (223234,'sanguinerite2',0,1,3000);
INSERT INTO `server_statuseffects` VALUES (223236,'outmaneuver2',524338,2,0);
INSERT INTO `server_statuseffects` VALUES (223237,'blindside2',8226,1,0);
INSERT INTO `server_statuseffects` VALUES (223238,'decoy2',4130,2,0);
INSERT INTO `server_statuseffects` VALUES (223239,'protect2',50,2,0);
INSERT INTO `server_statuseffects` VALUES (223240,'sanguinerite3',0,1,3000);
INSERT INTO `server_statuseffects` VALUES (223241,'bloodletter2',42,2,0);
INSERT INTO `server_statuseffects` VALUES (223242,'fully_blissful_mind',1073741858,1,0);
INSERT INTO `server_statuseffects` VALUES (223243,'magic_evasion_down',50,2,0);
INSERT INTO `server_statuseffects` VALUES (223245,'spinning_heel',50,1,0);
INSERT INTO `server_statuseffects` VALUES (223248,'divine_veil',36914,2,0);
INSERT INTO `server_statuseffects` VALUES (223250,'vengeance',16418,1,5000);
INSERT INTO `server_statuseffects` VALUES (223251,'antagonize',1048626,2,0);
INSERT INTO `server_statuseffects` VALUES (223264,'divine_regen',50,2,0);
INSERT INTO `server_statuseffects` VALUES (228021,'heavy',42,2,0);
INSERT INTO `server_statuseffects` VALUES (253003,'evade_proc',34,1,0);
INSERT INTO `server_statuseffects` VALUES (253004,'block_proc',34,1,0);
INSERT INTO `server_statuseffects` VALUES (253005,'parry_proc',34,1,0);
INSERT INTO `server_statuseffects` VALUES (253006,'miss_proc',34,1,0);
INSERT INTO `server_statuseffects` VALUES (253007,'expchain',34,1,0);
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

-- Dump completed on 2018-05-27 14:40:45
