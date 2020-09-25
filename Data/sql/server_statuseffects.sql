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
  `hidden` tinyint(4) NOT NULL DEFAULT '0',
  `silentOnGain` tinyint(4) NOT NULL DEFAULT '0',
  `silentOnLoss` tinyint(4) NOT NULL DEFAULT '0',
  `statusGainTextId` smallint(6) NOT NULL DEFAULT '30328',
  `statusLossTextId` int(11) NOT NULL DEFAULT '30331',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `server_statuseffects`
--

LOCK TABLES `server_statuseffects` WRITE;
/*!40000 ALTER TABLE `server_statuseffects` DISABLE KEYS */;
set autocommit=0;
INSERT INTO `server_statuseffects` VALUES (223001,'quick',9,2,0,0,0,0,30328,30331);
INSERT INTO `server_statuseffects` VALUES (223002,'haste',9,2,0,0,0,0,30328,30331);
INSERT INTO `server_statuseffects` VALUES (223003,'slow',21,2,0,0,0,0,30335,30338);
INSERT INTO `server_statuseffects` VALUES (223004,'petrification',264241173,2,0,0,0,0,30335,30338);
INSERT INTO `server_statuseffects` VALUES (223005,'paralysis',21,2,3000,0,0,0,30335,30338);
INSERT INTO `server_statuseffects` VALUES (223006,'silence',4194325,2,0,0,0,0,30335,30338);
INSERT INTO `server_statuseffects` VALUES (223007,'blind',21,2,0,0,0,0,30335,30338);
INSERT INTO `server_statuseffects` VALUES (223008,'mute',4194325,2,0,0,0,0,30335,30338);
INSERT INTO `server_statuseffects` VALUES (223009,'slowcast',517,1,0,0,0,0,30335,30338);
INSERT INTO `server_statuseffects` VALUES (223010,'glare',21,2,0,0,0,0,30335,30338);
INSERT INTO `server_statuseffects` VALUES (223011,'poison',21,2,0,0,0,0,30335,30338);
INSERT INTO `server_statuseffects` VALUES (223012,'transfixion',268435477,2,0,0,0,0,30335,30338);
INSERT INTO `server_statuseffects` VALUES (223013,'pacification',8388629,2,0,0,0,0,30335,30338);
INSERT INTO `server_statuseffects` VALUES (223014,'amnesia',16777237,2,0,0,0,0,30335,30338);
INSERT INTO `server_statuseffects` VALUES (223015,'stun',264241173,2,0,0,0,0,30335,30338);
INSERT INTO `server_statuseffects` VALUES (223016,'daze',264241173,2,0,0,0,0,30335,30338);
INSERT INTO `server_statuseffects` VALUES (223029,'hp_boost',9,2,0,0,0,0,30328,30331);
INSERT INTO `server_statuseffects` VALUES (223030,'hp_penalty',21,2,0,0,0,0,30335,30338);
INSERT INTO `server_statuseffects` VALUES (223038,'defense_down',21,2,0,0,0,0,30335,30338);
INSERT INTO `server_statuseffects` VALUES (223058,'aegis_boon',528409,2,0,0,0,0,30328,30331);
INSERT INTO `server_statuseffects` VALUES (223062,'sentinel',1048601,2,0,0,0,0,30328,30331);
INSERT INTO `server_statuseffects` VALUES (223063,'cover',16409,2,0,0,0,0,30328,30331);
INSERT INTO `server_statuseffects` VALUES (223064,'rampart',9,2,0,0,0,0,30328,30331);
INSERT INTO `server_statuseffects` VALUES (223068,'tempered_will',9,2,0,0,0,0,30328,30331);
INSERT INTO `server_statuseffects` VALUES (223075,'featherfoot',131097,2,0,0,0,0,30328,30331);
INSERT INTO `server_statuseffects` VALUES (223078,'enduring_march',9,2,0,0,0,0,30328,30331);
INSERT INTO `server_statuseffects` VALUES (223081,'bloodbath',1048601,2,0,0,0,0,30328,30331);
INSERT INTO `server_statuseffects` VALUES (223083,'foresight',262169,2,0,0,0,0,30328,30331);
INSERT INTO `server_statuseffects` VALUES (223091,'keen_flurry',1033,2,0,0,0,0,30328,30331);
INSERT INTO `server_statuseffects` VALUES (223094,'invigorate',9,2,0,0,0,0,30328,30331);
INSERT INTO `server_statuseffects` VALUES (223097,'collusion',1048601,1,0,0,0,0,30328,30331);
INSERT INTO `server_statuseffects` VALUES (223104,'quelling_strike',1041,2,0,0,0,0,30328,30331);
INSERT INTO `server_statuseffects` VALUES (223106,'hawks_eye',9,2,0,0,0,0,30328,30331);
INSERT INTO `server_statuseffects` VALUES (223108,'decoy',4113,2,0,0,0,0,30328,30331);
INSERT INTO `server_statuseffects` VALUES (223127,'bloodletter',21,2,0,0,0,0,30335,30338);
INSERT INTO `server_statuseffects` VALUES (223129,'protect',9,2,0,0,0,0,30328,30331);
INSERT INTO `server_statuseffects` VALUES (223133,'stoneskin',16393,1,0,0,0,0,30328,30331);
INSERT INTO `server_statuseffects` VALUES (223173,'covered',4105,2,0,0,0,0,30328,30331);
INSERT INTO `server_statuseffects` VALUES (223180,'regen',9,2,0,0,0,0,30328,30331);
INSERT INTO `server_statuseffects` VALUES (223181,'refresh',9,2,0,0,0,0,30328,30331);
INSERT INTO `server_statuseffects` VALUES (223182,'regain',9,2,0,0,0,0,30328,30331);
INSERT INTO `server_statuseffects` VALUES (223183,'tp_bleed',21,2,0,0,0,0,30335,30338);
INSERT INTO `server_statuseffects` VALUES (223205,'combo',21,2,0,0,1,0,0,30331);
INSERT INTO `server_statuseffects` VALUES (223206,'goring_blade',21,2,0,0,0,0,30335,30338);
INSERT INTO `server_statuseffects` VALUES (223207,'berserk2',537936145,1,0,0,0,0,30328,30331);
INSERT INTO `server_statuseffects` VALUES (223208,'rampage2',538984721,1,5000,0,0,0,30328,30331);
INSERT INTO `server_statuseffects` VALUES (223209,'fists_of_fire',536872209,2,0,0,0,0,30328,30331);
INSERT INTO `server_statuseffects` VALUES (223210,'fists_of_earth',536872209,2,0,0,0,0,30328,30331);
INSERT INTO `server_statuseffects` VALUES (223211,'fists_of_wind',536872209,2,0,0,0,0,30328,30331);
INSERT INTO `server_statuseffects` VALUES (223212,'power_surge_I',1297,2,0,0,0,0,30328,30331);
INSERT INTO `server_statuseffects` VALUES (223213,'power_surge_II',1297,2,0,0,0,0,30328,30331);
INSERT INTO `server_statuseffects` VALUES (223214,'power_surge_III',1297,2,0,0,0,0,30328,30331);
INSERT INTO `server_statuseffects` VALUES (223215,'life_surge_I',1048857,2,0,0,0,0,30328,30331);
INSERT INTO `server_statuseffects` VALUES (223216,'life_surge_II',1048857,2,0,0,0,0,30328,30331);
INSERT INTO `server_statuseffects` VALUES (223217,'life_surge_III',1048857,2,0,0,0,0,30328,30331);
INSERT INTO `server_statuseffects` VALUES (223218,'dread_spike',16649,2,0,0,0,0,30328,30331);
INSERT INTO `server_statuseffects` VALUES (223219,'blood_for_blood',8209,2,0,0,0,0,30328,30331);
INSERT INTO `server_statuseffects` VALUES (223220,'barrage',3081,2,0,0,0,0,30328,30331);
INSERT INTO `server_statuseffects` VALUES (223221,'raging_strike2',537985297,1,0,0,0,0,30328,30331);
INSERT INTO `server_statuseffects` VALUES (223224,'swiftsong',137,2,0,0,0,0,30328,30331);
INSERT INTO `server_statuseffects` VALUES (223227,'cleric_stance',8209,1,0,0,0,0,30328,30331);
INSERT INTO `server_statuseffects` VALUES (223228,'blissful_mind',536871185,1,1000,0,0,0,30328,30331);
INSERT INTO `server_statuseffects` VALUES (223229,'dark_seal2',1033,2,0,0,0,0,30328,30331);
INSERT INTO `server_statuseffects` VALUES (223230,'resonance2',2569,1,0,0,0,0,30328,30331);
INSERT INTO `server_statuseffects` VALUES (223231,'excruciate',2098185,2,3000,0,0,0,30328,30331);
INSERT INTO `server_statuseffects` VALUES (223232,'necrogenesis',1048585,1,0,0,0,0,30328,30331);
INSERT INTO `server_statuseffects` VALUES (223233,'parsimony',1049097,1,0,0,0,0,30328,30331);
INSERT INTO `server_statuseffects` VALUES (223234,'sanguine_rite2',16393,1,0,0,0,0,30328,30331);
INSERT INTO `server_statuseffects` VALUES (223235,'aero',21,2,0,0,0,0,30335,30338);
INSERT INTO `server_statuseffects` VALUES (223236,'outmaneuver2',524313,2,0,0,0,0,30328,30331);
INSERT INTO `server_statuseffects` VALUES (223237,'blindside2',8209,1,0,0,0,0,30328,30331);
INSERT INTO `server_statuseffects` VALUES (223238,'decoy2',4113,2,0,0,0,0,30328,30331);
INSERT INTO `server_statuseffects` VALUES (223239,'protect2',9,2,0,0,0,0,30328,30331);
INSERT INTO `server_statuseffects` VALUES (223240,'sanguine_rite3',16393,1,0,0,0,0,30328,30331);
INSERT INTO `server_statuseffects` VALUES (223241,'bloodletter2',21,2,0,0,0,0,30328,30331);
INSERT INTO `server_statuseffects` VALUES (223242,'fully_blissful_mind',536871185,1,0,0,0,0,30328,30331);
INSERT INTO `server_statuseffects` VALUES (223243,'magic_evasion_down',9,2,0,0,0,0,30335,30338);
INSERT INTO `server_statuseffects` VALUES (223244,'hundred_fists',257,2,0,0,0,0,30328,30331);
INSERT INTO `server_statuseffects` VALUES (223245,'spinning_heel',9,1,0,0,0,0,30328,30331);
INSERT INTO `server_statuseffects` VALUES (223248,'divine_veil',36889,2,0,0,0,0,30328,30331);
INSERT INTO `server_statuseffects` VALUES (223250,'vengeance',16401,1,5000,0,0,0,30328,30331);
INSERT INTO `server_statuseffects` VALUES (223251,'antagonize',1048601,2,0,0,0,0,30328,30331);
INSERT INTO `server_statuseffects` VALUES (223252,'mighty_strikes',8209,1,3000,0,0,0,30328,30331);
INSERT INTO `server_statuseffects` VALUES (223253,'battle_voice',9,2,0,0,0,0,30328,30331);
INSERT INTO `server_statuseffects` VALUES (223254,'ballad_of_magi',9,2,0,0,0,0,30328,30331);
INSERT INTO `server_statuseffects` VALUES (223255,'paeon_of_war',9,2,0,0,0,0,30328,30331);
INSERT INTO `server_statuseffects` VALUES (223256,'minuet_of_rigor',9,2,0,0,0,0,30328,30331);
INSERT INTO `server_statuseffects` VALUES (223264,'divine_regen',9,2,0,0,0,0,30328,30331);
INSERT INTO `server_statuseffects` VALUES (228001,'sleep',264273941,2,0,0,0,0,30335,30338);
INSERT INTO `server_statuseffects` VALUES (228011,'bind',67108885,2,0,0,0,0,30335,30338);
INSERT INTO `server_statuseffects` VALUES (228021,'heavy',21,2,0,0,0,0,30335,30338);
INSERT INTO `server_statuseffects` VALUES (300000,'evade_proc',273,1,0,1,1,1,0,30331);
INSERT INTO `server_statuseffects` VALUES (300001,'block_proc',273,1,0,1,1,1,0,30331);
INSERT INTO `server_statuseffects` VALUES (300002,'parry_proc',273,1,0,1,1,1,0,30331);
INSERT INTO `server_statuseffects` VALUES (300003,'miss_proc',273,1,0,1,1,1,0,30331);
INSERT INTO `server_statuseffects` VALUES (300004,'expchain',273,1,0,1,1,1,0,30331);
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

-- Dump completed on 2019-06-05 18:56:07
