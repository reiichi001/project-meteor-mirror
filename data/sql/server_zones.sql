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
-- Table structure for table `server_zones`
--

DROP TABLE IF EXISTS `server_zones`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `server_zones` (
  `id` int(10) unsigned NOT NULL,
  `regionId` smallint(6) unsigned NOT NULL,
  `zoneName` varchar(255) DEFAULT NULL,
  `placeName` varchar(255) NOT NULL,
  `serverIp` varchar(32) NOT NULL,
  `serverPort` int(10) unsigned NOT NULL,
  `classPath` varchar(255) NOT NULL,
  `dayMusic` smallint(6) unsigned DEFAULT '0',
  `nightMusic` smallint(6) unsigned DEFAULT '0',
  `battleMusic` smallint(6) unsigned DEFAULT '0',
  `isIsolated` tinyint(1) DEFAULT '0',
  `isInn` tinyint(1) DEFAULT '0',
  `canRideChocobo` tinyint(1) DEFAULT '1',
  `canStealth` tinyint(1) DEFAULT '0',
  `isInstanceRaid` tinyint(1) unsigned DEFAULT '0',
  `loadNavMesh` tinyint(1) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `server_zones`
--

LOCK TABLES `server_zones` WRITE;
/*!40000 ALTER TABLE `server_zones` DISABLE KEYS */;
set autocommit=0;
INSERT INTO `server_zones` VALUES (0,0,NULL,'--','127.0.0.1',1989,'',0,0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (128,101,'sea0Field01','Lower La Noscea','127.0.0.1',1989,'/Area/Zone/ZoneMasterSeaS0',60,60,21,0,0,1,0,0,0);
INSERT INTO `server_zones` VALUES (129,101,'sea0Field02','Western La Noscea','127.0.0.1',1989,'/Area/Zone/ZoneMasterSeaS0',60,60,21,0,0,1,0,0,0);
INSERT INTO `server_zones` VALUES (130,101,'sea0Field03','Eastern La Noscea','127.0.0.1',1989,'/Area/Zone/ZoneMasterSeaS0',60,60,21,0,0,1,0,0,0);
INSERT INTO `server_zones` VALUES (131,101,'sea0Dungeon01','Mistbeard Cove','127.0.0.1',1989,'/Area/Zone/ZoneMasterSeaS0',0,0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (132,101,'sea0Dungeon03','Cassiopeia Hollow','127.0.0.1',1989,'/Area/Zone/ZoneMasterSeaS0',0,0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (133,101,'sea0Town01','Limsa Lominsa','127.0.0.1',1989,'/Area/Zone/ZoneMasterSeaS0',59,59,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (134,202,'sea0Market01','Market Wards','127.0.0.1',1989,'/Area/Zone/ZoneMasterMarketSeaS0',0,0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (135,101,'sea0Field04','Upper La Noscea','127.0.0.1',1989,'/Area/Zone/ZoneMasterSeaS0',60,60,21,0,0,1,0,0,0);
INSERT INTO `server_zones` VALUES (137,101,'sea0Dungeon06','U\'Ghamaro Mines','127.0.0.1',1989,'/Area/Zone/ZoneMasterSeaS0',0,0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (138,101,NULL,'La Noscea','127.0.0.1',1989,'',60,60,21,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (139,112,'sea0Field01a','The Cieldalaes','127.0.0.1',1989,'/Area/Zone/ZoneMasterSeaS0',0,0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (140,101,NULL,'Sailors Ward','127.0.0.1',1989,'',0,0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (141,101,'sea0Field01a','Lower La Noscea','127.0.0.1',1989,'/Area/Zone/ZoneMasterSeaS0',60,60,21,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (143,102,'roc0Field01','Coerthas Central Highlands','127.0.0.1',1989,'/Area/Zone/ZoneMasterRocR0',55,55,15,0,0,1,0,0,0);
INSERT INTO `server_zones` VALUES (144,102,'roc0Field02','Coerthas Eastern Highlands','127.0.0.1',1989,'/Area/Zone/ZoneMasterRocR0',55,55,15,0,0,1,0,0,0);
INSERT INTO `server_zones` VALUES (145,102,'roc0Field03','Coerthas Eastern Lowlands','127.0.0.1',1989,'/Area/Zone/ZoneMasterRocR0',55,55,15,0,0,1,0,0,0);
INSERT INTO `server_zones` VALUES (146,102,NULL,'Coerthas','127.0.0.1',1989,'',55,55,15,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (147,102,'roc0Field04','Coerthas Central Lowlands','127.0.0.1',1989,'/Area/Zone/ZoneMasterRocR0',55,55,15,0,0,1,0,0,0);
INSERT INTO `server_zones` VALUES (148,102,'roc0Field05','Coerthas Western Highlands','127.0.0.1',1989,'/Area/Zone/ZoneMasterRocR0',55,55,15,0,0,1,0,0,0);
INSERT INTO `server_zones` VALUES (150,103,'fst0Field01','Central Shroud','127.0.0.1',1989,'/Area/Zone/ZoneMasterFstF0',52,52,13,0,0,1,0,0,0);
INSERT INTO `server_zones` VALUES (151,103,'fst0Field02','East Shroud','127.0.0.1',1989,'/Area/Zone/ZoneMasterFstF0',52,52,13,0,0,1,0,0,0);
INSERT INTO `server_zones` VALUES (152,103,'fst0Field03','North Shroud','127.0.0.1',1989,'/Area/Zone/ZoneMasterFstF0',52,52,13,0,0,1,0,0,0);
INSERT INTO `server_zones` VALUES (153,103,'fst0Field04','West Shroud','127.0.0.1',1989,'/Area/Zone/ZoneMasterFstF0',52,52,13,0,0,1,0,0,0);
INSERT INTO `server_zones` VALUES (154,103,'fst0Field05','South Shroud','127.0.0.1',1989,'/Area/Zone/ZoneMasterFstF0',52,52,13,0,0,1,0,0,0);
INSERT INTO `server_zones` VALUES (155,103,'fst0Town01','Gridania','127.0.0.1',1989,'/Area/Zone/ZoneMasterFstF0',51,51,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (156,103,NULL,'The Black Shroud','127.0.0.1',1989,'',0,0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (157,103,'fst0Dungeon01','The Mun-Tuy Cellars','127.0.0.1',1989,'/Area/Zone/ZoneMasterFstF0',0,0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (158,103,'fst0Dungeon02','The Tam-Tara Deepcroft','127.0.0.1',1989,'/Area/Zone/ZoneMasterFstF0',0,0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (159,103,'fst0Dungeon03','The Thousand Maws of Toto-Rak','127.0.0.1',1989,'/Area/Zone/ZoneMasterFstF0',0,0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (160,204,'fst0Market01','Market Wards','127.0.0.1',1989,'/Area/Zone/ZoneMasterMarketFstF0',0,0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (161,103,NULL,'Peasants Ward','127.0.0.1',1989,'',0,0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (162,103,'fst0Field01a','Central Shroud','127.0.0.1',1989,'/Area/Zone/ZoneMasterFstF0',52,52,13,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (164,106,'fst0Battle01','Central Shroud','127.0.0.1',1989,'/Area/Zone/ZoneMasterBattleFstF0',0,0,13,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (165,106,'fst0Battle02','Central Shroud','127.0.0.1',1989,'/Area/Zone/ZoneMasterBattleFstF0',0,0,13,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (166,106,'fst0Battle03','Central Shroud','127.0.0.1',1989,'/Area/Zone/ZoneMasterBattleFstF0',0,0,13,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (167,106,'fst0Battle04','Central Shroud','127.0.0.1',1989,'/Area/Zone/ZoneMasterBattleFstF0',0,0,13,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (168,106,'fst0Battle05','Central Shroud','127.0.0.1',1989,'/Area/Zone/ZoneMasterBattleFstF0',0,0,13,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (170,104,'wil0Field01','Central Thanalan','127.0.0.1',1989,'/Area/Zone/ZoneMasterWilW0',68,68,25,0,0,1,0,0,0);
INSERT INTO `server_zones` VALUES (171,104,'wil0Field02','Eastern Thanalan','127.0.0.1',1989,'/Area/Zone/ZoneMasterWilW0',68,68,25,0,0,1,0,0,0);
INSERT INTO `server_zones` VALUES (172,104,'wil0Field03','Western Thanalan','127.0.0.1',1989,'/Area/Zone/ZoneMasterWilW0',68,68,25,0,0,1,0,0,0);
INSERT INTO `server_zones` VALUES (173,104,'wil0Field04','Northern Thanalan','127.0.0.1',1989,'/Area/Zone/ZoneMasterWilW0',68,68,25,0,0,1,0,0,0);
INSERT INTO `server_zones` VALUES (174,104,'wil0Field05','Southern Thanalan','127.0.0.1',1989,'/Area/Zone/ZoneMasterWilW0',68,68,25,0,0,1,0,0,0);
INSERT INTO `server_zones` VALUES (175,104,'wil0Town01','Ul\'dah','127.0.0.1',1989,'/Area/Zone/ZoneMasterWilW0',66,66,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (176,104,'wil0Dungeon02','Nanawa Mines','127.0.0.1',1989,'/Area/Zone/ZoneMasterWilW0',0,0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (177,207,'_jail','-','127.0.0.1',1989,'/Area/Zone/ZoneMasterJail',0,0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (178,104,'wil0Dungeon04','Copperbell Mines','127.0.0.1',1989,'/Area/Zone/ZoneMasterWilW0',0,0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (179,104,NULL,'Thanalan','127.0.0.1',1989,'',0,0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (180,205,'wil0Market01','Market Wards','127.0.0.1',1989,'/Area/Zone/ZoneMasterMarketWilW0',0,0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (181,104,NULL,'Merchants Ward','127.0.0.1',1989,'',0,0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (182,104,NULL,'Central Thanalan','127.0.0.1',1989,'',68,68,25,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (184,107,'wil0Battle01','Ul\'dah','127.0.0.1',1989,'/Area/Zone/ZoneMasterBattleWilW0',0,0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (185,107,'wil0Battle01','Ul\'dah','127.0.0.1',1989,'/Area/Zone/ZoneMasterBattleWilW0',0,0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (186,104,'wil0Battle02','Ul\'dah','127.0.0.1',1989,'/Area/Zone/ZoneMasterBattleWilW0',0,0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (187,104,'wil0Battle03','Ul\'dah','127.0.0.1',1989,'/Area/Zone/ZoneMasterBattleWilW0',0,0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (188,104,'wil0Battle04','Ul\'dah','127.0.0.1',1989,'/Area/Zone/ZoneMasterBattleWilW0',0,0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (190,105,'lak0Field01','Mor Dhona','127.0.0.1',1989,'/Area/Zone/ZoneMasterLakL0',49,49,11,0,0,1,0,0,0);
INSERT INTO `server_zones` VALUES (192,112,'ocn1Battle01','Rhotano Sea','127.0.0.1',1989,'/Area/Zone/ZoneMasterBattleOcnO1',0,0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (193,111,'ocn0Battle02','Rhotano Sea','127.0.0.1',1989,'/Area/Zone/ZoneMasterBattleOcnO0',7,0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (194,112,'ocn1Battle03','Rhotano Sea','127.0.0.1',1989,'/Area/Zone/ZoneMasterBattleOcnO1',0,0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (195,112,'ocn1Battle04','Rhotano Sea','127.0.0.1',1989,'/Area/Zone/ZoneMasterBattleOcnO1',0,0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (196,112,'ocn1Battle05','Rhotano Sea','127.0.0.1',1989,'/Area/Zone/ZoneMasterBattleOcnO1',0,0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (198,112,'ocn1Battle06','Rhotano Sea','127.0.0.1',1989,'/Area/Zone/ZoneMasterBattleOcnO1',0,0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (200,805,'sea1Cruise01','Strait of Merlthor','127.0.0.1',1989,'/Area/Zone/ZoneMasterCruiseSeaS1',65,0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (201,208,'prv0Cottage00','-','127.0.0.1',1989,'/Area/Zone/ZoneMasterCottagePrv00',0,0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (204,101,'sea0Field02a','Western La Noscea','127.0.0.1',1989,'',60,60,21,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (205,101,'sea0Field03a','Eastern La Noscea','127.0.0.1',1989,'',60,60,21,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (206,103,'fst0Town01a','Gridania','127.0.0.1',1989,'/Area/Zone/ZoneMasterFstF0',51,51,13,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (207,103,'fst0Field03a','North Shroud','127.0.0.1',1989,'/Area/Zone/ZoneMasterFstF0',52,52,13,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (208,103,'fst0Field05a','South Shroud','127.0.0.1',1989,'/Area/Zone/ZoneMasterFstF0',52,52,13,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (209,104,'wil0Town01a','Ul\'dah','127.0.0.1',1989,'/Area/Zone/ZoneMasterWilW0',66,66,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (210,104,NULL,'Eastern Thanalan','127.0.0.1',1989,'',68,68,25,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (211,104,NULL,'Western Thanalan','127.0.0.1',1989,'',68,68,25,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (230,101,'sea0Town01a','Limsa Lominsa','127.0.0.1',1989,'/Area/Zone/ZoneMasterSeaS0',59,59,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (231,102,'roc0Dungeon01','Dzemael Darkhold','127.0.0.1',1989,'/Area/Zone/ZoneMasterRocR0',0,0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (232,202,'sea0Office01','Maelstrom Command','127.0.0.1',1989,'/Area/Zone/ZoneMasterOfficeSeaS0',3,0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (233,205,'wil0Office01','Hall of Flames','127.0.0.1',1989,'/Area/Zone/ZoneMasterOfficeWilW0',4,0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (234,204,'fst0Office01','Adders\' Nest','127.0.0.1',1989,'/Area/Zone/ZoneMasterOfficeFstF0',2,0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (235,101,NULL,'Shposhae','127.0.0.1',1989,'',0,0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (236,101,'sea1Field01','Locke\'s Lie','127.0.0.1',1989,'/Area/Zone/ZoneMasterSeaS1',0,0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (237,101,NULL,'Turtleback Island','127.0.0.1',1989,'',0,0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (238,103,'fst0Field04','Thornmarch','127.0.0.1',1989,'',0,0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (239,102,'roc0Field02a','The Howling Eye','127.0.0.1',1989,'/Area/Zone/ZoneMasterRocR0',0,0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (240,104,'wil0Field05a','The Bowl of Embers','127.0.0.1',1989,'',0,0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (244,209,'prv0Inn01','Inn Room','127.0.0.1',1989,'/Area/Zone/ZoneMasterPrvI0',61,61,0,0,1,0,0,0,0);
INSERT INTO `server_zones` VALUES (245,102,'roc0Dungeon04','The Aurum Vale','127.0.0.1',1989,'/Area/Zone/ZoneMasterRocR0',0,0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (246,104,NULL,'Cutter\'s Cry','127.0.0.1',1989,'',0,0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (247,103,NULL,'North Shroud','127.0.0.1',1989,'',0,0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (248,101,NULL,'Western La Noscea','127.0.0.1',1989,'',0,0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (249,104,NULL,'Eastern Thanalan','127.0.0.1',1989,'',0,0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (250,102,'roc0Field02a','The Howling Eye','127.0.0.1',1989,'/Area/Zone/ZoneMasterRocR0',0,0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (251,105,NULL,'Transmission Tower','127.0.0.1',1989,'',0,0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (252,102,'roc0Dungeon04','The Aurum Vale','127.0.0.1',1989,'/Area/Zone/ZoneMasterRocR0',0,0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (253,102,'roc0Dungeon04','The Aurum Vale','127.0.0.1',1989,'/Area/Zone/ZoneMasterRocR0',0,0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (254,104,NULL,'Cutter\'s Cry','127.0.0.1',1989,'',0,0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (255,104,NULL,'Cutter\'s Cry','127.0.0.1',1989,'',0,0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (256,102,'roc0Field02a','The Howling Eye','127.0.0.1',1989,'/Area/Zone/ZoneMasterRocR0',0,0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (257,109,'roc1Field01','Rivenroad','127.0.0.1',1989,'/Area/Zone/ZoneMasterRocR1',0,0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (258,103,NULL,'North Shroud','127.0.0.1',1989,'',0,0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (259,103,NULL,'North Shroud','127.0.0.1',1989,'',0,0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (260,101,NULL,'Western La Noscea','127.0.0.1',1989,'',0,0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (261,101,NULL,'Western La Noscea','127.0.0.1',1989,'',0,0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (262,104,NULL,'Eastern Thanalan','127.0.0.1',1989,'',0,0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (263,104,NULL,'Eastern Thanalan','127.0.0.1',1989,'',0,0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (264,105,'lak0Field01','Transmission Tower','127.0.0.1',1989,'',0,0,0,0,0,1,0,0,0);
INSERT INTO `server_zones` VALUES (265,104,NULL,'The Bowl of Embers','127.0.0.1',1989,'',0,0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (266,105,'lak0Field01a','Mor Dhona','127.0.0.1',1989,'/Area/Zone/ZoneMasterLakL0',49,49,11,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (267,109,'roc1Field02','Rivenroad','127.0.0.1',1989,'/Area/Zone/ZoneMasterRocR1',0,0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (268,109,'roc1Field03','Rivenroad','127.0.0.1',1989,'/Area/Zone/ZoneMasterRocR1',0,0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (269,101,NULL,'Locke\'s Lie','127.0.0.1',1989,'',0,0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (270,101,NULL,'Turtleback Island','127.0.0.1',1989,'',0,0,0,0,0,0,0,0,0);
/*!40000 ALTER TABLE `server_zones` ENABLE KEYS */;
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

-- Dump completed on 2017-06-29 18:47:53
