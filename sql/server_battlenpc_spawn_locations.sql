/*
MySQL Data Transfer
Source Host: localhost
Source Database: ffxiv_server
Target Host: localhost
Target Database: ffxiv_server
Date: 7/9/2017 7:11:04 PM
*/
DROP TABLE IF EXISTS `server_battlenpc_spawn_locations`;

SET FOREIGN_KEY_CHECKS=0;
SET AUTOCOMMIT=0;
-- ----------------------------
-- Table structure for server_battlenpc_spawn_locations
-- ----------------------------
CREATE TABLE `server_battlenpc_spawn_locations` (
  `uniqueId` varchar(32) NOT NULL DEFAULT '',
  `customDisplayName` varchar(32) DEFAULT NULL,
  `groupId` int(10) unsigned NOT NULL,
  `positionX` float NOT NULL,
  `positionY` float NOT NULL,
  `positionZ` float NOT NULL,
  `rotation` float NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;
