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
-- Table structure for table `server_battlenpc_genus`
--

DROP TABLE IF EXISTS `server_battlenpc_genus`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `server_battlenpc_genus` (
  `genusId` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `name` varchar(255) NOT NULL,
  `modelSize` tinyint(3) unsigned NOT NULL DEFAULT '1',
  `speed` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `kindredId` int(11) unsigned NOT NULL DEFAULT '0',
  `kindredName` varchar(255) NOT NULL DEFAULT 'Unknown',
  `detection` smallint(5) unsigned NOT NULL DEFAULT '0',
  `hpp` smallint(5) unsigned NOT NULL DEFAULT '100',
  `mpp` smallint(5) unsigned NOT NULL DEFAULT '100',
  `tpp` smallint(5) unsigned NOT NULL DEFAULT '100',
  `str` smallint(4) unsigned NOT NULL DEFAULT '1',
  `vit` smallint(4) unsigned NOT NULL DEFAULT '1',
  `dex` smallint(4) unsigned NOT NULL DEFAULT '1',
  `int` smallint(4) unsigned NOT NULL DEFAULT '1',
  `mnd` smallint(4) unsigned NOT NULL DEFAULT '1',
  `pie` smallint(4) unsigned NOT NULL DEFAULT '1',
  `att` smallint(4) unsigned NOT NULL DEFAULT '1',
  `acc` smallint(4) unsigned NOT NULL DEFAULT '1',
  `def` smallint(4) unsigned NOT NULL DEFAULT '1',
  `eva` smallint(4) unsigned NOT NULL DEFAULT '1',
  `slash` float NOT NULL DEFAULT '1',
  `pierce` float NOT NULL DEFAULT '1',
  `h2h` float NOT NULL DEFAULT '1',
  `blunt` float NOT NULL DEFAULT '1',
  `fire` float NOT NULL DEFAULT '1',
  `ice` float NOT NULL DEFAULT '1',
  `wind` float NOT NULL DEFAULT '1',
  `lightning` float NOT NULL DEFAULT '1',
  `earth` float NOT NULL DEFAULT '1',
  `water` float NOT NULL DEFAULT '1',
  `element` tinyint(4) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`genusId`)
) ENGINE=InnoDB AUTO_INCREMENT=66 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `server_battlenpc_genus`
--

LOCK TABLES `server_battlenpc_genus` WRITE;
/*!40000 ALTER TABLE `server_battlenpc_genus` DISABLE KEYS */;
set autocommit=0;
INSERT INTO `server_battlenpc_genus` VALUES (1,'Aldgoat',1,8,1,'Beast',1,100,100,100,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0);
INSERT INTO `server_battlenpc_genus` VALUES (2,'Antelope',1,8,1,'Beast',1,100,100,100,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0);
INSERT INTO `server_battlenpc_genus` VALUES (3,'Wolf',1,8,1,'Beast',2,100,100,100,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0);
INSERT INTO `server_battlenpc_genus` VALUES (4,'Opo-opo',1,8,1,'Beast',1,100,100,100,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0);
INSERT INTO `server_battlenpc_genus` VALUES (5,'Coeurl',1,8,1,'Beast',15,100,100,100,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0);
INSERT INTO `server_battlenpc_genus` VALUES (6,'Goobbue',1,8,1,'Beast',4,100,100,100,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0);
INSERT INTO `server_battlenpc_genus` VALUES (7,'Sheep',1,8,1,'Beast',1,100,100,100,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0);
INSERT INTO `server_battlenpc_genus` VALUES (8,'Buffalo',1,8,1,'Beast',4,100,100,100,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0);
INSERT INTO `server_battlenpc_genus` VALUES (9,'Boar',1,8,1,'Beast',2,100,100,100,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0);
INSERT INTO `server_battlenpc_genus` VALUES (10,'Moon-Mouse?',1,8,1,'Beast',2,100,100,100,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0);
INSERT INTO `server_battlenpc_genus` VALUES (11,'Mole',1,8,1,'Beast',4,100,100,100,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0);
INSERT INTO `server_battlenpc_genus` VALUES (12,'Rodent',1,8,1,'Beast',2,100,100,100,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0);
INSERT INTO `server_battlenpc_genus` VALUES (13,'Cactuar',1,8,2,'Plantoid',0,100,100,100,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0);
INSERT INTO `server_battlenpc_genus` VALUES (14,'Funguar',1,8,2,'Plantoid',0,100,100,100,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0);
INSERT INTO `server_battlenpc_genus` VALUES (15,'Flying-trap',1,8,2,'Plantoid',0,100,100,100,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0);
INSERT INTO `server_battlenpc_genus` VALUES (16,'Morbol',1,8,2,'Plantoid',0,100,100,100,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0);
INSERT INTO `server_battlenpc_genus` VALUES (17,'Orobon',1,8,3,'Aquan',0,100,100,100,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0);
INSERT INTO `server_battlenpc_genus` VALUES (18,'Gigantoad',1,8,3,'Aquan',0,100,100,100,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0);
INSERT INTO `server_battlenpc_genus` VALUES (19,'Salamander',1,8,3,'Aquan',0,100,100,100,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0);
INSERT INTO `server_battlenpc_genus` VALUES (20,'Jelly-fish',1,8,3,'Aquan',0,100,100,100,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0);
INSERT INTO `server_battlenpc_genus` VALUES (21,'Slug',1,8,3,'Aquan',0,100,100,100,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0);
INSERT INTO `server_battlenpc_genus` VALUES (22,'Megalo-crab',1,8,3,'Aquan',0,100,100,100,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0);
INSERT INTO `server_battlenpc_genus` VALUES (23,'Amaalja',1,8,4,'Spoken',0,100,100,100,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0);
INSERT INTO `server_battlenpc_genus` VALUES (24,'Ixal',1,8,4,'Spoken',0,100,100,100,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0);
INSERT INTO `server_battlenpc_genus` VALUES (25,'Qiqirn',1,8,4,'Spoken',0,100,100,100,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0);
INSERT INTO `server_battlenpc_genus` VALUES (26,'Goblin',1,8,4,'Spoken',0,100,100,100,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0);
INSERT INTO `server_battlenpc_genus` VALUES (27,'Kobold',1,8,4,'Spoken',0,100,100,100,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0);
INSERT INTO `server_battlenpc_genus` VALUES (28,'Sylph',1,8,4,'Spoken',0,100,100,100,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0);
INSERT INTO `server_battlenpc_genus` VALUES (29,'Person',1,8,4,'Spoken',0,100,100,100,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0);
INSERT INTO `server_battlenpc_genus` VALUES (30,'Drake',1,8,5,'Reptilian',0,100,100,100,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0);
INSERT INTO `server_battlenpc_genus` VALUES (31,'Basilisk',1,8,5,'Reptilian',0,100,100,100,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0);
INSERT INTO `server_battlenpc_genus` VALUES (32,'Raptor',1,8,5,'Reptilian',0,100,100,100,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0);
INSERT INTO `server_battlenpc_genus` VALUES (33,'Ant-ring',1,8,6,'Insect',0,100,100,100,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0);
INSERT INTO `server_battlenpc_genus` VALUES (34,'Swarm',1,8,6,'Insect',0,100,100,100,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0);
INSERT INTO `server_battlenpc_genus` VALUES (35,'Diremite',1,8,6,'Insect',0,100,100,100,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0);
INSERT INTO `server_battlenpc_genus` VALUES (36,'Chigoe',1,8,6,'Insect',0,100,100,100,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0);
INSERT INTO `server_battlenpc_genus` VALUES (37,'Gnat',1,8,6,'Insect',0,100,100,100,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0);
INSERT INTO `server_battlenpc_genus` VALUES (38,'Beetle',1,8,6,'Insect',0,100,100,100,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0);
INSERT INTO `server_battlenpc_genus` VALUES (39,'Yarzon',1,8,6,'Insect',0,100,100,100,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0);
INSERT INTO `server_battlenpc_genus` VALUES (40,'Apkallu',1,8,7,'Avian',0,100,100,100,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0);
INSERT INTO `server_battlenpc_genus` VALUES (41,'Vulture',1,8,7,'Avian',0,100,100,100,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0);
INSERT INTO `server_battlenpc_genus` VALUES (42,'Dodo',1,8,7,'Avian',0,100,100,100,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0);
INSERT INTO `server_battlenpc_genus` VALUES (43,'Bat',1,8,7,'Avian',0,100,100,100,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0);
INSERT INTO `server_battlenpc_genus` VALUES (44,'Hippogryph',1,8,7,'Avian',0,100,100,100,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0);
INSERT INTO `server_battlenpc_genus` VALUES (45,'Puk',1,8,7,'Avian',0,100,100,100,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0);
INSERT INTO `server_battlenpc_genus` VALUES (46,'Ghost',1,8,8,'Undead',0,100,100,100,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0);
INSERT INTO `server_battlenpc_genus` VALUES (47,'The-Damned',1,8,8,'Undead',0,100,100,100,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0);
INSERT INTO `server_battlenpc_genus` VALUES (48,'Wight',1,8,8,'Undead',0,100,100,100,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0);
INSERT INTO `server_battlenpc_genus` VALUES (49,'Coblyn',1,8,9,'Cursed',0,100,100,100,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0);
INSERT INTO `server_battlenpc_genus` VALUES (50,'Spriggan',1,8,9,'Cursed',0,100,100,100,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0);
INSERT INTO `server_battlenpc_genus` VALUES (51,'Ahriman',1,8,10,'Voidsent',0,100,100,100,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0);
INSERT INTO `server_battlenpc_genus` VALUES (52,'Imp',1,8,10,'Voidsent',0,100,100,100,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0);
INSERT INTO `server_battlenpc_genus` VALUES (53,'Will-O-Wisp',1,8,10,'Voidsent',0,100,100,100,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0);
INSERT INTO `server_battlenpc_genus` VALUES (54,'Fire-Elemental',1,8,10,'Voidsent',0,100,100,100,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0);
INSERT INTO `server_battlenpc_genus` VALUES (55,'Water-Elemental',1,8,10,'Voidsent',0,100,100,100,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0);
INSERT INTO `server_battlenpc_genus` VALUES (56,'Earth-Elemental',1,8,10,'Voidsent',0,100,100,100,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0);
INSERT INTO `server_battlenpc_genus` VALUES (57,'Lightning-Elemental',1,8,10,'Voidsent',0,100,100,100,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0);
INSERT INTO `server_battlenpc_genus` VALUES (58,'Ice-Elemental',1,8,10,'Voidsent',0,100,100,100,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0);
INSERT INTO `server_battlenpc_genus` VALUES (59,'Wind-Elemental',1,8,10,'Voidsent',0,100,100,100,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0);
INSERT INTO `server_battlenpc_genus` VALUES (60,'Ogre',1,8,10,'Voidsent',0,100,100,100,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0);
INSERT INTO `server_battlenpc_genus` VALUES (61,'Phurble',1,8,10,'Voidsent',0,100,100,100,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0);
INSERT INTO `server_battlenpc_genus` VALUES (62,'Plasmoid',1,8,10,'Voidsent',0,100,100,100,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0);
INSERT INTO `server_battlenpc_genus` VALUES (63,'Flan',1,8,10,'Voidsent',0,100,100,100,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0);
INSERT INTO `server_battlenpc_genus` VALUES (64,'Bomb',1,8,10,'Voidsent',0,100,100,100,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0);
INSERT INTO `server_battlenpc_genus` VALUES (65,'Grenade',1,8,10,'Voidsent',0,100,100,100,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0);
/*!40000 ALTER TABLE `server_battlenpc_genus` ENABLE KEYS */;
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

-- Dump completed on 2017-10-11 14:48:52
