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
-- Table structure for table `server_battle_commands`
--

DROP TABLE IF EXISTS `server_battle_commands`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `server_battle_commands` (
  `id` smallint(5) unsigned NOT NULL,
  `name` varchar(255) NOT NULL,
  `classJob` tinyint(3) unsigned NOT NULL,
  `lvl` tinyint(3) unsigned NOT NULL,
  `requirements` smallint(5) unsigned NOT NULL,
  `validTarget` tinyint(3) unsigned NOT NULL,
  `aoeType` tinyint(3) unsigned NOT NULL,
  `aoeRange` int(10) NOT NULL DEFAULT '0',
  `aoeTarget` tinyint(3) NOT NULL,
  `numHits` tinyint(3) unsigned NOT NULL,
  `positionBonus` tinyint(3) unsigned NOT NULL,
  `procRequirement` tinyint(3) unsigned NOT NULL,
  `range` int(10) unsigned NOT NULL,
  `buffDuration` int(10) unsigned NOT NULL,
  `debuffDuration` int(10) unsigned NOT NULL,
  `castType` tinyint(3) unsigned NOT NULL,
  `castTime` int(10) unsigned NOT NULL,
  `recastTime` int(10) unsigned NOT NULL,
  `mpCost` smallint(5) unsigned NOT NULL,
  `tpCost` smallint(5) unsigned NOT NULL,
  `animationType` tinyint(3) unsigned NOT NULL,
  `effectAnimation` smallint(5) unsigned NOT NULL,
  `modelAnimation` smallint(5) unsigned NOT NULL,
  `animationDuration` smallint(5) unsigned NOT NULL,
  `battleAnimation` int(10) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `server_battle_commands`
--

LOCK TABLES `server_battle_commands` WRITE;
/*!40000 ALTER TABLE `server_battle_commands` DISABLE KEYS */;
set autocommit=0;
INSERT INTO `server_battle_commands` VALUES (27100,'second_wind',2,6,3,1,0,0,0,1,0,0,5,0,0,0,0,45,0,0,14,519,2,2,234889735);
INSERT INTO `server_battle_commands` VALUES (27101,'blindside',2,14,3,1,0,0,0,1,0,0,5,0,60,0,0,60,0,0,14,635,2,2,234889851);
INSERT INTO `server_battle_commands` VALUES (27102,'taunt',2,42,4,32,0,0,0,1,0,0,5,5,0,0,0,60,0,0,14,517,2,2,234889733);
INSERT INTO `server_battle_commands` VALUES (27103,'featherfoot',2,2,3,1,0,0,0,1,0,0,5,0,30,0,0,60,0,0,14,535,2,2,234889751);
INSERT INTO `server_battle_commands` VALUES (27104,'fists_of_fire',2,34,4,1,0,0,0,1,0,0,5,0,0,0,0,10,0,0,14,684,2,2,234889900);
INSERT INTO `server_battle_commands` VALUES (27105,'fists_of_earth',2,22,4,1,0,0,0,1,0,0,5,0,0,0,0,10,0,0,14,685,2,2,234889901);
INSERT INTO `server_battle_commands` VALUES (27106,'hundred_fists',15,50,0,0,0,0,0,1,0,0,5,0,0,0,0,900,0,0,14,712,2,2,234889928);
INSERT INTO `server_battle_commands` VALUES (27107,'spinning_heel',15,35,0,0,0,0,0,1,0,0,5,0,0,0,0,120,0,0,14,718,2,2,234889934);
INSERT INTO `server_battle_commands` VALUES (27108,'shoulder_tackle',15,30,0,0,0,0,0,1,0,0,5,0,0,0,0,60,0,0,18,1048,205,2,302830616);
INSERT INTO `server_battle_commands` VALUES (27109,'fists_of_wind',15,40,0,0,0,0,0,1,0,0,5,0,0,0,0,10,0,0,14,720,2,2,234889936);
INSERT INTO `server_battle_commands` VALUES (27110,'pummel',2,1,1,32,0,0,0,1,1,0,5,0,0,0,0,10,0,1000,18,1027,1,2,301995011);
INSERT INTO `server_battle_commands` VALUES (27111,'concussive_blow',2,10,1,32,0,0,0,1,4,0,5,30,0,0,0,30,0,1500,18,20,3,2,302002196);
INSERT INTO `server_battle_commands` VALUES (27112,'simian_thrash',2,50,4,32,0,0,0,1,0,0,5,0,0,0,0,80,0,2000,18,1003,202,2,302818283);
INSERT INTO `server_battle_commands` VALUES (27113,'aura_pulse',2,38,4,32,1,0,0,1,0,0,5,30,0,0,0,40,0,1500,18,66,203,2,302821442);
INSERT INTO `server_battle_commands` VALUES (27114,'pounce',2,4,4,32,0,0,0,1,2,0,5,10,0,0,0,20,0,1500,18,8,3,2,302002184);
INSERT INTO `server_battle_commands` VALUES (27115,'demolish',2,30,1,32,0,0,0,1,0,0,5,0,0,0,0,30,0,1500,18,1028,2,2,301999108);
INSERT INTO `server_battle_commands` VALUES (27116,'howling_fist',2,46,4,32,0,0,0,1,4,0,5,0,0,0,0,80,0,3000,18,1029,2,2,301999109);
INSERT INTO `server_battle_commands` VALUES (27117,'sucker_punch',2,26,1,32,0,0,0,1,4,0,5,0,0,0,0,15,0,1000,18,73,3,2,302002249);
INSERT INTO `server_battle_commands` VALUES (27118,'dragon_kick',15,45,0,0,0,0,0,1,0,0,5,0,0,0,0,60,0,2000,18,1041,204,2,302826513);
INSERT INTO `server_battle_commands` VALUES (27119,'haymaker',2,18,4,32,0,0,0,1,0,1,5,0,0,0,0,5,0,250,18,23,201,2,302813207);
INSERT INTO `server_battle_commands` VALUES (27140,'sentinel',3,22,3,1,0,0,0,1,0,0,5,0,15,0,0,90,0,0,14,526,2,2,234889742);
INSERT INTO `server_battle_commands` VALUES (27141,'aegis_boon',3,6,8,1,0,0,0,1,0,0,5,0,30,0,0,60,0,0,14,583,21,2,234967623);
INSERT INTO `server_battle_commands` VALUES (27142,'rampart',3,2,3,1,0,0,0,1,0,2,5,0,60,0,0,120,0,0,14,536,2,2,234889752);
INSERT INTO `server_battle_commands` VALUES (27143,'tempered_will',3,42,8,1,0,0,0,1,0,0,5,0,20,0,0,180,0,0,14,515,2,2,234889731);
INSERT INTO `server_battle_commands` VALUES (27144,'outmaneuver',3,34,8,1,0,0,0,1,0,0,5,0,30,0,0,90,0,0,14,512,21,2,234967552);
INSERT INTO `server_battle_commands` VALUES (27145,'flash',3,14,3,32,0,0,0,1,0,0,5,0,0,0,0,30,0,0,14,696,2,2,234889912);
INSERT INTO `server_battle_commands` VALUES (27146,'cover',16,30,0,0,0,0,0,1,0,0,5,0,15,0,0,60,0,0,14,725,2,2,234889941);
INSERT INTO `server_battle_commands` VALUES (27147,'divine_veil',16,35,0,0,0,0,0,1,0,0,5,0,20,0,0,60,0,0,14,713,2,2,234889929);
INSERT INTO `server_battle_commands` VALUES (27148,'hallowed_ground',16,50,0,0,0,0,0,1,0,0,5,0,0,0,0,900,0,0,14,709,2,2,234889925);
INSERT INTO `server_battle_commands` VALUES (27149,'holy_succor',16,40,0,0,0,0,0,1,0,0,15,0,0,0,2,10,100,0,1,701,1,2,16782013);
INSERT INTO `server_battle_commands` VALUES (27150,'fast_blade',3,1,1,32,0,0,0,1,1,0,5,0,0,0,0,10,0,1000,18,1023,1,2,301995007);
INSERT INTO `server_battle_commands` VALUES (27151,'flat_blade',3,26,1,32,0,0,0,1,0,0,5,0,0,0,0,10,0,1500,18,1024,2,2,301999104);
INSERT INTO `server_battle_commands` VALUES (27152,'savage_blade',3,10,1,32,0,0,0,1,0,0,5,0,0,0,0,30,0,1000,18,1025,1,2,301995009);
INSERT INTO `server_battle_commands` VALUES (27153,'goring_blade',3,50,8,32,0,0,0,1,2,0,5,30,0,0,0,60,0,3000,18,1026,301,2,303223810);
INSERT INTO `server_battle_commands` VALUES (27154,'riot_blade',3,30,8,32,0,0,0,1,2,0,5,30,0,0,0,80,0,2000,18,75,2,2,301998155);
INSERT INTO `server_battle_commands` VALUES (27155,'rage_of_halone',3,46,8,32,0,0,0,1,0,0,5,0,0,0,0,20,0,1500,18,1008,302,2,303227888);
INSERT INTO `server_battle_commands` VALUES (27156,'shield_bash',3,18,17,32,0,0,0,1,0,0,5,5,0,0,0,30,0,250,18,5,26,2,302096389);
INSERT INTO `server_battle_commands` VALUES (27157,'war_drum',3,38,24,32,1,0,0,1,0,2,5,0,0,0,0,60,0,500,14,502,21,2,234967542);
INSERT INTO `server_battle_commands` VALUES (27158,'phalanx',3,4,8,1,0,0,0,1,0,0,5,0,0,0,0,5,0,250,18,32,1,2,301994016);
INSERT INTO `server_battle_commands` VALUES (27159,'spirits_within',16,45,0,0,0,0,0,1,0,0,5,0,0,0,0,60,0,3000,18,1044,304,2,303236116);
INSERT INTO `server_battle_commands` VALUES (27180,'provoke',4,14,3,32,0,0,0,1,0,0,5,0,0,0,0,30,0,0,14,600,2,2,234889816);
INSERT INTO `server_battle_commands` VALUES (27181,'foresight',4,2,3,1,0,0,0,1,0,0,5,0,30,0,0,60,0,0,14,545,2,2,234889761);
INSERT INTO `server_battle_commands` VALUES (27182,'bloodbath',4,6,3,1,0,0,0,1,0,0,5,0,30,0,0,60,0,0,14,581,2,2,234889797);
INSERT INTO `server_battle_commands` VALUES (27183,'berserk',4,22,32,1,0,0,0,1,0,0,5,0,0,0,0,10,0,0,14,682,2,2,234889898);
INSERT INTO `server_battle_commands` VALUES (27184,'rampage',4,34,32,1,0,0,0,1,0,0,5,0,0,0,0,10,0,0,14,546,2,2,234889762);
INSERT INTO `server_battle_commands` VALUES (27185,'enduring_march',4,42,32,1,0,0,0,1,0,0,5,0,20,0,0,180,0,0,14,539,2,2,234889755);
INSERT INTO `server_battle_commands` VALUES (27186,'vengeance',17,30,0,1,0,0,0,1,0,0,5,0,0,0,0,150,0,0,14,714,2,2,234889930);
INSERT INTO `server_battle_commands` VALUES (27187,'antagonize',17,35,0,1,0,0,0,1,0,0,5,0,0,0,0,120,0,0,14,715,2,2,234889931);
INSERT INTO `server_battle_commands` VALUES (27188,'collusion',17,40,0,4,0,0,0,1,0,0,5,0,0,0,0,90,0,0,14,711,2,2,234889927);
INSERT INTO `server_battle_commands` VALUES (27189,'mighty_strikes',17,50,0,1,0,0,0,1,0,0,5,0,0,0,0,900,0,0,14,716,2,2,234889932);
INSERT INTO `server_battle_commands` VALUES (27190,'heavy_swing',4,1,1,32,0,0,0,1,1,0,5,0,0,0,0,10,0,1000,18,14,1,2,301993998);
INSERT INTO `server_battle_commands` VALUES (27191,'skull_sunder',4,10,1,32,0,0,0,1,0,0,5,0,0,0,0,30,0,1500,18,43,1,2,301994027);
INSERT INTO `server_battle_commands` VALUES (27192,'steel_cyclone',17,45,0,32,1,0,0,1,0,0,5,0,0,0,0,30,0,2000,18,1040,404,2,303645712);
INSERT INTO `server_battle_commands` VALUES (27193,'brutal_swing',4,4,1,32,0,0,0,1,4,0,5,0,0,0,0,20,0,1500,18,15,3,2,302002191);
INSERT INTO `server_battle_commands` VALUES (27194,'maim',4,26,1,32,0,0,0,1,0,0,5,0,0,0,0,30,0,1500,18,88,1,2,301994072);
INSERT INTO `server_battle_commands` VALUES (27195,'godsbane',4,50,32,32,0,0,0,1,0,0,5,0,0,0,0,60,0,3000,18,1014,402,2,303637494);
INSERT INTO `server_battle_commands` VALUES (27196,'path_of_the_storm',4,38,32,32,0,0,0,1,2,0,5,0,0,0,0,30,0,1500,18,44,401,2,303632428);
INSERT INTO `server_battle_commands` VALUES (27197,'whirlwind',4,46,32,32,1,0,0,1,0,0,5,0,0,0,0,80,0,3000,18,1015,403,2,303641591);
INSERT INTO `server_battle_commands` VALUES (27198,'fracture',4,18,32,32,0,0,0,1,0,4,5,8,0,0,0,40,0,500,18,42,3,2,302002218);
INSERT INTO `server_battle_commands` VALUES (27199,'overpower',4,30,1,32,2,0,0,1,0,4,5,0,0,0,0,5,0,250,18,89,1,2,301994073);
INSERT INTO `server_battle_commands` VALUES (27220,'hawks_eye',7,6,3,1,0,0,0,1,0,0,5,0,15,0,0,90,0,0,14,516,2,2,234889732);
INSERT INTO `server_battle_commands` VALUES (27221,'quelling_strikes',7,22,3,1,0,0,0,1,0,0,5,30,0,0,0,60,0,0,14,614,2,2,234889830);
INSERT INTO `server_battle_commands` VALUES (27222,'decoy',7,2,3,1,0,0,0,1,0,0,15,0,60,0,0,90,100,0,14,565,2,2,234889781);
INSERT INTO `server_battle_commands` VALUES (27223,'chameleon',7,42,3,1,0,0,0,1,0,0,5,0,0,0,0,180,0,0,14,504,2,2,234889720);
INSERT INTO `server_battle_commands` VALUES (27224,'barrage',7,34,64,1,0,0,0,1,0,0,5,0,60,0,0,90,0,0,14,683,2,2,234889899);
INSERT INTO `server_battle_commands` VALUES (27225,'raging_strikes',7,14,64,1,0,0,0,1,0,0,5,0,0,0,0,10,0,0,14,632,2,2,234889848);
INSERT INTO `server_battle_commands` VALUES (27226,'swiftsong',7,26,64,1,1,0,0,1,0,0,15,0,180,0,0,10,100,0,1,150,1,2,16781462);
INSERT INTO `server_battle_commands` VALUES (27227,'battle_voice',18,50,0,1,1,0,0,1,0,0,5,0,0,0,0,900,0,0,14,721,2,2,234889937);
INSERT INTO `server_battle_commands` VALUES (27228,'heavy_shot',7,1,1,32,0,0,0,1,0,0,5,0,0,0,0,10,0,1000,18,1036,4,2,302007308);
INSERT INTO `server_battle_commands` VALUES (27229,'leaden_arrow',7,10,1,32,0,0,0,1,0,0,5,30,0,0,0,30,0,1500,18,1035,4,2,302007307);
INSERT INTO `server_battle_commands` VALUES (27230,'wide_volley',7,50,64,32,1,0,0,1,0,0,5,0,0,0,0,80,0,2000,18,18,703,2,304869394);
INSERT INTO `server_battle_commands` VALUES (27231,'quick_nock',7,38,64,32,2,0,0,1,0,0,5,0,0,0,0,180,0,1000,18,1017,702,2,304866297);
INSERT INTO `server_battle_commands` VALUES (27232,'rain_of_death',18,45,0,32,1,0,0,1,0,0,5,0,0,0,0,30,0,3000,18,1037,704,2,304874509);
INSERT INTO `server_battle_commands` VALUES (27233,'piercing_arrow',7,4,1,32,0,0,0,1,0,0,5,0,0,0,0,20,0,1000,18,1038,1,2,301995022);
INSERT INTO `server_battle_commands` VALUES (27234,'gloom_arrow',7,30,1,32,0,0,0,1,0,0,5,30,0,0,0,10,0,1000,18,1039,4,2,302007311);
INSERT INTO `server_battle_commands` VALUES (27235,'bloodletter',7,46,64,32,0,0,0,1,0,0,5,30,0,0,0,80,0,1500,18,53,701,2,304861237);
INSERT INTO `server_battle_commands` VALUES (27236,'shadowbind',7,18,64,32,0,0,0,1,0,0,5,30,0,0,0,40,0,250,18,17,4,2,302006289);
INSERT INTO `server_battle_commands` VALUES (27237,'ballad_of_magi',18,30,0,1,1,0,0,1,0,0,15,0,0,0,3,10,100,0,1,709,1,2,16782021);
INSERT INTO `server_battle_commands` VALUES (27238,'paeon_of_war',18,40,0,1,1,0,0,1,0,0,15,0,0,0,3,10,50,1000,1,710,1,2,16782022);
INSERT INTO `server_battle_commands` VALUES (27239,'minuet_of_rigor',18,35,0,1,1,0,0,1,0,0,15,0,0,0,3,10,100,0,1,711,1,2,16782023);
INSERT INTO `server_battle_commands` VALUES (27258,'refill',7,1,0,1,0,0,0,1,0,0,5,0,0,0,0,0,0,0,0,0,0,2,0);
INSERT INTO `server_battle_commands` VALUES (27259,'light_shot',7,1,0,32,0,0,0,1,0,0,5,0,0,0,0,0,0,0,0,0,0,2,0);
INSERT INTO `server_battle_commands` VALUES (27260,'invigorate',8,14,3,1,0,0,0,1,0,0,5,0,30,0,0,90,0,0,14,575,2,2,234889791);
INSERT INTO `server_battle_commands` VALUES (27261,'power_surge',8,34,128,1,0,0,0,1,0,0,5,0,0,0,0,10,0,0,14,686,2,2,234889902);
INSERT INTO `server_battle_commands` VALUES (27262,'life_surge',8,22,128,1,0,0,0,1,0,0,5,0,0,0,0,15,0,250,14,687,2,2,234889903);
INSERT INTO `server_battle_commands` VALUES (27263,'dread_spike',8,42,128,1,0,0,0,1,0,0,5,0,0,0,0,120,0,0,14,686,2,2,234889902);
INSERT INTO `server_battle_commands` VALUES (27264,'blood_for_blood',8,6,3,1,0,0,0,1,0,0,5,0,0,0,0,60,0,0,14,689,2,2,234889905);
INSERT INTO `server_battle_commands` VALUES (27265,'keen_flurry',8,26,3,1,0,0,0,1,0,0,5,0,0,0,0,90,0,0,14,569,2,2,234889785);
INSERT INTO `server_battle_commands` VALUES (27266,'jump',19,30,0,32,0,0,0,1,0,0,5,0,0,0,0,60,0,0,18,1045,804,2,305284117);
INSERT INTO `server_battle_commands` VALUES (27267,'elusive_jump',19,40,0,1,0,0,0,1,0,0,5,0,0,0,0,180,0,0,18,1046,806,2,305292310);
INSERT INTO `server_battle_commands` VALUES (27268,'dragonfire_dive',19,50,0,32,1,0,0,1,0,0,5,0,0,0,0,900,0,0,18,1045,804,2,305284117);
INSERT INTO `server_battle_commands` VALUES (27269,'true_thrust',8,1,1,32,0,0,0,1,1,0,5,0,0,0,0,10,0,1000,18,1030,2,2,301999110);
INSERT INTO `server_battle_commands` VALUES (27270,'leg_sweep',8,30,1,32,1,0,0,1,0,0,5,8,0,0,0,30,0,1000,18,37,1,2,301994021);
INSERT INTO `server_battle_commands` VALUES (27271,'doom_spike',8,46,128,32,3,0,0,1,0,0,5,0,0,0,0,60,0,3000,18,83,801,2,305270867);
INSERT INTO `server_battle_commands` VALUES (27272,'disembowel',19,35,0,32,0,0,0,1,0,0,5,0,0,0,0,30,0,750,18,1042,2,2,301999122);
INSERT INTO `server_battle_commands` VALUES (27273,'heavy_thrust',8,10,1,32,0,0,0,1,0,0,5,4,0,0,0,20,0,1500,18,1031,1,2,301995015);
INSERT INTO `server_battle_commands` VALUES (27274,'vorpal_thrust',8,2,1,32,0,0,0,1,2,0,5,0,0,0,0,20,0,1500,18,1032,2,2,301999112);
INSERT INTO `server_battle_commands` VALUES (27275,'impulse_drive',8,18,1,32,0,0,0,1,4,0,5,0,0,0,0,30,0,1500,18,1033,2,2,301999113);
INSERT INTO `server_battle_commands` VALUES (27276,'chaos_thrust',8,50,128,32,0,0,0,1,0,0,5,0,0,0,0,80,0,3000,18,40,802,2,305274920);
INSERT INTO `server_battle_commands` VALUES (27277,'ring_of_talons',19,45,0,32,1,0,0,1,0,0,5,0,0,0,0,60,0,2000,18,1009,803,2,305279985);
INSERT INTO `server_battle_commands` VALUES (27278,'feint',8,4,1,32,0,0,0,1,0,8,5,0,0,0,0,10,0,250,18,39,2,2,301998119);
INSERT INTO `server_battle_commands` VALUES (27279,'full_thrust',8,38,128,32,0,0,0,1,0,8,5,0,0,0,0,30,0,250,18,1034,801,2,305271818);
INSERT INTO `server_battle_commands` VALUES (27300,'dark_seal',22,14,3,1,0,0,0,1,0,0,5,0,30,0,0,90,0,0,14,518,2,2,234889734);
INSERT INTO `server_battle_commands` VALUES (27301,'resonance',22,22,3,1,0,0,0,1,0,0,5,0,30,0,0,90,0,0,14,669,2,2,234889885);
INSERT INTO `server_battle_commands` VALUES (27302,'excruciate',22,38,256,1,0,0,0,1,0,0,5,0,30,0,0,90,0,0,14,694,2,2,234889910);
INSERT INTO `server_battle_commands` VALUES (27303,'necrogenesis',22,6,3,1,0,0,0,1,0,0,5,0,30,0,0,90,0,0,14,695,2,2,234889911);
INSERT INTO `server_battle_commands` VALUES (27304,'parsimony',22,2,256,1,0,0,0,1,0,0,5,0,30,0,0,90,0,0,14,568,2,2,234889784);
INSERT INTO `server_battle_commands` VALUES (27305,'convert',26,30,0,1,0,0,0,1,0,0,5,0,0,0,0,450,0,0,14,724,2,2,234889940);
INSERT INTO `server_battle_commands` VALUES (27306,'sleep',22,42,256,32,0,0,0,1,0,0,15,60,0,0,3,0,75,0,1,651,1,2,16781963);
INSERT INTO `server_battle_commands` VALUES (27307,'sanguine_rite',22,30,3,1,0,0,0,1,0,0,15,0,20,0,0,60,120,0,1,152,1,2,16781464);
INSERT INTO `server_battle_commands` VALUES (27308,'blizzard',22,4,256,32,0,0,0,1,0,0,15,30,0,0,3,10,90,0,1,502,1,2,16781814);
INSERT INTO `server_battle_commands` VALUES (27309,'blizzara',22,26,256,32,1,0,0,1,0,0,15,30,0,0,0,40,150,0,1,506,1,2,16781818);
INSERT INTO `server_battle_commands` VALUES (27310,'fire',22,10,3,32,1,0,0,1,0,0,15,0,0,0,3,8,105,0,1,501,1,2,16781813);
INSERT INTO `server_battle_commands` VALUES (27311,'fira',22,34,3,32,1,0,0,1,0,0,15,0,0,0,5,16,180,0,1,504,1,2,16781816);
INSERT INTO `server_battle_commands` VALUES (27312,'firaga',22,50,256,32,1,0,0,1,0,0,15,0,0,0,8,7,255,0,1,700,1,2,16782012);
INSERT INTO `server_battle_commands` VALUES (27313,'thunder',22,1,3,32,0,0,0,1,0,0,15,0,0,0,2,6,75,0,1,503,1,2,16781815);
INSERT INTO `server_battle_commands` VALUES (27314,'thundara',22,18,256,32,0,0,0,1,0,0,15,4,0,0,0,30,135,0,1,508,1,2,16781820);
INSERT INTO `server_battle_commands` VALUES (27315,'thundaga',22,46,256,32,0,0,0,1,0,0,15,0,0,0,5,45,195,0,1,509,1,2,16781821);
INSERT INTO `server_battle_commands` VALUES (27316,'burst',26,50,0,32,0,0,0,1,0,0,15,0,0,0,4,900,90,0,1,705,1,2,16782017);
INSERT INTO `server_battle_commands` VALUES (27317,'sleepga',26,45,0,32,1,0,0,1,0,0,15,0,0,0,4,0,100,0,1,704,1,2,16782016);
INSERT INTO `server_battle_commands` VALUES (27318,'flare',26,40,0,32,1,0,0,1,0,0,15,0,0,0,8,120,200,0,1,706,1,2,16782018);
INSERT INTO `server_battle_commands` VALUES (27319,'freeze',26,35,0,32,0,0,0,1,0,0,15,0,0,0,5,120,120,0,1,707,1,2,16782019);
INSERT INTO `server_battle_commands` VALUES (27340,'sacred_prism',23,34,3,1,0,0,0,1,0,0,5,0,60,0,0,90,0,0,14,690,2,2,234889906);
INSERT INTO `server_battle_commands` VALUES (27341,'shroud_of_saints',23,38,512,1,0,0,0,1,0,0,5,0,20,0,0,180,0,0,14,691,2,2,234889907);
INSERT INTO `server_battle_commands` VALUES (27342,'cleric_stance',23,10,512,1,0,0,0,1,0,0,5,0,0,0,0,30,0,0,14,692,2,2,234889908);
INSERT INTO `server_battle_commands` VALUES (27343,'blissful_mind',23,14,512,1,0,0,0,1,0,0,5,0,0,0,0,30,0,0,14,693,2,2,234889909);
INSERT INTO `server_battle_commands` VALUES (27344,'presence_of_mind',27,30,0,1,0,0,0,1,0,0,5,0,0,0,0,300,0,0,14,722,2,2,234889938);
INSERT INTO `server_battle_commands` VALUES (27345,'benediction',27,50,0,1,1,0,0,1,0,0,5,0,0,0,0,900,0,0,14,723,2,2,234889939);
INSERT INTO `server_battle_commands` VALUES (27346,'cure',23,2,3,2,0,0,0,1,0,0,15,0,0,0,2,5,40,0,1,101,1,2,16781413);
INSERT INTO `server_battle_commands` VALUES (27347,'cura',23,30,512,2,0,0,0,1,0,0,15,0,0,0,2,5,100,0,1,103,1,2,16781415);
INSERT INTO `server_battle_commands` VALUES (27348,'curaga',23,46,512,4,1,0,0,1,0,0,15,0,0,0,3,10,150,0,1,146,1,2,16781458);
INSERT INTO `server_battle_commands` VALUES (27349,'raise',23,18,512,2,0,0,0,1,0,0,15,0,0,0,10,300,150,0,1,148,1,2,16781460);
INSERT INTO `server_battle_commands` VALUES (27350,'stoneskin',23,26,3,2,0,0,0,1,0,0,15,0,300,0,3,30,50,0,1,133,1,2,16781445);
INSERT INTO `server_battle_commands` VALUES (27351,'protect',23,6,3,4,1,0,0,1,0,0,15,0,300,0,3,30,80,0,1,1085,1,2,16782397);
INSERT INTO `server_battle_commands` VALUES (27352,'repose',23,50,0,32,0,0,0,1,0,0,15,0,0,0,3,0,80,0,1,151,1,2,16781463);
INSERT INTO `server_battle_commands` VALUES (27353,'aero',23,4,3,32,0,0,0,1,0,0,15,0,0,0,3,6,75,0,1,510,1,2,16781822);
INSERT INTO `server_battle_commands` VALUES (27354,'aerora',23,42,512,32,1,0,0,1,0,0,15,0,0,0,4,20,150,0,1,511,1,2,16781823);
INSERT INTO `server_battle_commands` VALUES (27355,'stone',23,1,3,32,0,0,0,1,0,0,15,0,0,0,2,6,75,0,1,513,1,2,16781825);
INSERT INTO `server_battle_commands` VALUES (27356,'stonera',23,22,512,32,1,0,0,1,0,0,15,0,0,0,3,30,150,0,1,514,1,2,16781826);
INSERT INTO `server_battle_commands` VALUES (27357,'esuna',27,40,0,2,0,0,0,1,0,0,15,0,0,0,2,10,40,0,1,702,1,2,16782014);
INSERT INTO `server_battle_commands` VALUES (27358,'regen',27,35,0,2,0,0,0,1,0,0,15,0,0,0,2,5,20,0,1,703,1,2,16782015);
INSERT INTO `server_battle_commands` VALUES (27359,'holy',27,45,0,32,1,0,0,1,0,0,15,0,0,0,0,300,100,0,1,708,1,2,16782020);
/*!40000 ALTER TABLE `server_battle_commands` ENABLE KEYS */;
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

-- Dump completed on 2017-08-31  5:52:52
