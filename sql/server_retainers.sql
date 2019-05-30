/*
MySQL Data Transfer
Source Host: localhost
Source Database: ffxiv_server
Target Host: localhost
Target Database: ffxiv_server
Date: 9/9/2017 2:30:21 PM
*/

SET FOREIGN_KEY_CHECKS=0;
-- ----------------------------
-- Table structure for server_retainers
-- ----------------------------
CREATE TABLE `server_retainers` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `name` varchar(255) NOT NULL,
  `actorClassId` int(10) unsigned NOT NULL,
  `cdIDOffset` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `placeName` smallint(5) unsigned NOT NULL,
  `conditions` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `level` tinyint(3) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8;
