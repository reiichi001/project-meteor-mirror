-- MySQL dump 10.13  Distrib 5.7.23, for Win64 (x86_64)
--
-- Host: localhost    Database: ffxiv_server
-- ------------------------------------------------------
-- Server version	5.7.23

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
-- Table structure for table `server_battle_traits`
--

DROP TABLE IF EXISTS `server_battle_traits`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `server_battle_traits` (
  `id` smallint(6) NOT NULL,
  `name` varchar(50) NOT NULL,
  `classJob` tinyint(4) NOT NULL,
  `lvl` tinyint(4) NOT NULL,
  `modifier` int(11) NOT NULL DEFAULT '0',
  `bonus` smallint(6) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `server_battle_traits`
--

LOCK TABLES `server_battle_traits` WRITE;
/*!40000 ALTER TABLE `server_battle_traits` DISABLE KEYS */;
set autocommit=0;
INSERT INTO `server_battle_traits` VALUES (27240,'enhanced_hawks_eye',7,28,0,0);
INSERT INTO `server_battle_traits` VALUES (27242,'enhanced_barrage',7,44,0,0);
INSERT INTO `server_battle_traits` VALUES (27241,'enhanced_quelling_strike',7,32,0,0);
INSERT INTO `server_battle_traits` VALUES (27243,'enhanced_raging_strike',7,36,0,0);
INSERT INTO `server_battle_traits` VALUES (27244,'enhanced_decoy',7,16,0,0);
INSERT INTO `server_battle_traits` VALUES (27245,'swift_chameleon',7,48,0,0);
INSERT INTO `server_battle_traits` VALUES (27246,'enhanced_physical_crit_accuracy',7,40,19,10);
INSERT INTO `server_battle_traits` VALUES (27247,'enhanced_physical_crit_evasion',7,20,20,10);
INSERT INTO `server_battle_traits` VALUES (27248,'enhanced_physical_evasion',7,12,16,8);
INSERT INTO `server_battle_traits` VALUES (27249,'enhanced_physical_accuracy',7,8,15,8);
INSERT INTO `server_battle_traits` VALUES (27250,'enhanced_physical_accuracy_ii',7,24,15,10);
INSERT INTO `server_battle_traits` VALUES (27120,'enhanced_second_wind',2,20,0,0);
INSERT INTO `server_battle_traits` VALUES (27121,'enhanced_blindside',2,24,0,0);
INSERT INTO `server_battle_traits` VALUES (27122,'swift_taunt',2,48,0,0);
INSERT INTO `server_battle_traits` VALUES (27123,'enhanced_featherfoot',2,28,0,0);
INSERT INTO `server_battle_traits` VALUES (27124,'enhanced_fists_of_fire',2,44,0,0);
INSERT INTO `server_battle_traits` VALUES (27125,'enhanced_fists_of_earth',2,36,0,0);
INSERT INTO `server_battle_traits` VALUES (27126,'enhanced_physical_accuracy',2,16,15,8);
INSERT INTO `server_battle_traits` VALUES (27127,'enhanced_physical_attack',2,8,17,8);
INSERT INTO `server_battle_traits` VALUES (27128,'enhanced_physical_attack_ii',2,40,17,10);
INSERT INTO `server_battle_traits` VALUES (27129,'enhanced_evasion',2,12,16,8);
INSERT INTO `server_battle_traits` VALUES (27130,'enhanced_physical_crit_damage',2,32,21,10);
INSERT INTO `server_battle_traits` VALUES (27160,'enhanced_sentinel',3,36,0,0);
INSERT INTO `server_battle_traits` VALUES (27161,'enhanced_flash',3,28,0,0);
INSERT INTO `server_battle_traits` VALUES (27162,'enhanced_flash_ii',3,48,0,0);
INSERT INTO `server_battle_traits` VALUES (27163,'enhanced_rampart',3,12,0,0);
INSERT INTO `server_battle_traits` VALUES (27164,'swift_aegis_boon',3,20,0,0);
INSERT INTO `server_battle_traits` VALUES (27165,'enhanced_outmaneuver',3,44,0,0);
INSERT INTO `server_battle_traits` VALUES (27167,'enhanced_block_rate',3,16,41,10);
INSERT INTO `server_battle_traits` VALUES (27166,'enhanced_physical_crit_resilience',3,32,22,10);
INSERT INTO `server_battle_traits` VALUES (27168,'enhanced_physical_defense',3,8,18,10);
INSERT INTO `server_battle_traits` VALUES (27169,'enhanced_physical_defense_ii',3,24,18,10);
INSERT INTO `server_battle_traits` VALUES (27170,'enhanced_physical_defense_iii',3,40,18,12);
INSERT INTO `server_battle_traits` VALUES (27200,'enhanced_provoke',4,28,0,0);
INSERT INTO `server_battle_traits` VALUES (27201,'swift_foresight',4,20,0,0);
INSERT INTO `server_battle_traits` VALUES (27202,'swift_bloodbath',4,16,0,0);
INSERT INTO `server_battle_traits` VALUES (27203,'enhanced_enduring_march',4,48,0,0);
INSERT INTO `server_battle_traits` VALUES (27204,'enhanced_rampage',4,44,0,0);
INSERT INTO `server_battle_traits` VALUES (27205,'enhanced_berserk',4,36,0,0);
INSERT INTO `server_battle_traits` VALUES (27206,'enhanced_physical_crit_evasion',4,32,20,10);
INSERT INTO `server_battle_traits` VALUES (27207,'enhanced_parry',4,24,39,8);
INSERT INTO `server_battle_traits` VALUES (27208,'enhanced_physical_defense',4,12,18,8);
INSERT INTO `server_battle_traits` VALUES (27209,'enhanced_physical_defense_ii',4,40,18,10);
INSERT INTO `server_battle_traits` VALUES (27210,'enhanced_physical_attack_power',4,8,17,8);
INSERT INTO `server_battle_traits` VALUES (27280,'enhanced_invigorate',8,28,0,0);
INSERT INTO `server_battle_traits` VALUES (27281,'enhanced_power_surge',8,44,0,0);
INSERT INTO `server_battle_traits` VALUES (27282,'enhanced_life_surge',8,32,0,0);
INSERT INTO `server_battle_traits` VALUES (27283,'enhanced_blood_for_blood',8,48,0,0);
INSERT INTO `server_battle_traits` VALUES (27284,'swift_blood_for_blood',8,16,0,0);
INSERT INTO `server_battle_traits` VALUES (27285,'enhanced_keen_flurry',8,36,0,0);
INSERT INTO `server_battle_traits` VALUES (27286,'store_tp',8,12,50,50);
INSERT INTO `server_battle_traits` VALUES (27287,'enhanced_physical_crit_accuracy',8,24,19,10);
INSERT INTO `server_battle_traits` VALUES (27288,'enhanced_physical_attack_power',8,8,17,8);
INSERT INTO `server_battle_traits` VALUES (27289,'enhanced_physical_attack_power_ii',8,20,17,10);
INSERT INTO `server_battle_traits` VALUES (27290,'enhanced_physical_attack_power_iii',8,40,17,10);
INSERT INTO `server_battle_traits` VALUES (27320,'swift_dark_seal',22,36,0,0);
INSERT INTO `server_battle_traits` VALUES (27321,'enhanced_excruciate',22,48,0,0);
INSERT INTO `server_battle_traits` VALUES (27322,'swift_necrogenesis',22,24,0,0);
INSERT INTO `server_battle_traits` VALUES (27323,'enhanced_parsimony',22,16,0,0);
INSERT INTO `server_battle_traits` VALUES (27324,'enhanced_sanguine_rite',22,44,0,0);
INSERT INTO `server_battle_traits` VALUES (27325,'enhanced_enfeebling_magic',22,12,26,8);
INSERT INTO `server_battle_traits` VALUES (27326,'enhanced_enfeebling_magic_ii',22,28,26,10);
INSERT INTO `server_battle_traits` VALUES (27327,'enhanced_magic_potency',22,8,23,8);
INSERT INTO `server_battle_traits` VALUES (27328,'enhanced_magic_potency_ii',22,28,23,10);
INSERT INTO `server_battle_traits` VALUES (27329,'enhanced_magic_crit_potency',22,40,37,10);
INSERT INTO `server_battle_traits` VALUES (27330,'auto-refresh',22,20,49,3);
INSERT INTO `server_battle_traits` VALUES (27360,'swift_sacred_prism',23,40,0,0);
INSERT INTO `server_battle_traits` VALUES (27361,'swift_shroud_of_saints',23,44,0,0);
INSERT INTO `server_battle_traits` VALUES (27362,'enhanced_blissful_mind',23,32,0,0);
INSERT INTO `server_battle_traits` VALUES (27363,'enhanced_raise',23,48,0,0);
INSERT INTO `server_battle_traits` VALUES (27364,'enhanced_stoneskin',23,36,0,0);
INSERT INTO `server_battle_traits` VALUES (27365,'enhanced_protect',23,24,0,0);
INSERT INTO `server_battle_traits` VALUES (27366,'greater_enhancing_magic',23,12,25,8);
INSERT INTO `server_battle_traits` VALUES (27367,'greater_healing',23,8,24,8);
INSERT INTO `server_battle_traits` VALUES (27368,'greater_healing_ii',23,18,24,10);
INSERT INTO `server_battle_traits` VALUES (27369,'enhanced_magic_accuracy',23,16,27,8);
INSERT INTO `server_battle_traits` VALUES (27370,'auto-refresh',23,20,49,3);
/*!40000 ALTER TABLE `server_battle_traits` ENABLE KEYS */;
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

-- Dump completed on 2019-06-01 21:15:51
