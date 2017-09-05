/*
MySQL Data Transfer
Source Host: localhost
Source Database: ffxiv_server
Target Host: localhost
Target Database: ffxiv_server
Date: 5/1/2017 10:28:15 PM
*/

SET FOREIGN_KEY_CHECKS=0;
-- ----------------------------
-- Table structure for server_battlenpc_groups
-- ----------------------------
DROP TABLE IF EXISTS `server_battlenpc_groups`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `server_battlenpc_groups` (
  `groupId` int(10) unsigned NOT NULL DEFAULT '0',
  `genusId` int(10) unsigned NOT NULL DEFAULT '0',
  `actorClassId` int(10) unsigned NOT NULL,
  `minLevel` tinyint(3) unsigned NOT NULL DEFAULT '1',
  `maxLevel` tinyint(3) unsigned NOT NULL DEFAULT '1',
  `respawnTime` int(10) unsigned NOT NULL DEFAULT '10',
  `hp` int(10) unsigned NOT NULL DEFAULT '0',
  `mp` int(10) unsigned NOT NULL DEFAULT '0',
  `skillListId` int(10) unsigned NOT NULL DEFAULT '0',
  `spellListId` int(10) unsigned NOT NULL DEFAULT '0',
  `dropListId` int(10) unsigned NOT NULL DEFAULT '0',
  `allegiance` int(10) unsigned NOT NULL DEFAULT '0',
  `spawnType` smallint(5) unsigned NOT NULL DEFAULT '0',
  `animationId` int(10) unsigned NOT NULL DEFAULT '0',
  `actorState` smallint(5) unsigned NOT NULL DEFAULT '0',
  `privateAreaName` varchar(32) NOT NULL DEFAULT '',
  `privateAreaLevel` int(11) NOT NULL DEFAULT '0',
  `zoneId` smallint(3) unsigned NOT NULL,
  PRIMARY KEY (`groupId`)
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8;
