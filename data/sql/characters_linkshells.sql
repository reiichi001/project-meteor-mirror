/*
MySQL Data Transfer
Source Host: localhost
Source Database: ffxiv_server
Target Host: localhost
Target Database: ffxiv_server
Date: 4/2/2017 3:12:27 PM
*/

SET FOREIGN_KEY_CHECKS=0;
-- ----------------------------
-- Table structure for characters_linkshells
-- ----------------------------
CREATE TABLE `characters_linkshells` (
  `characterId` int(10) unsigned NOT NULL,
  `linkshellId` bigint(20) unsigned NOT NULL,
  `rank` tinyint(3) unsigned NOT NULL DEFAULT '4',
  PRIMARY KEY (`characterId`,`linkshellId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
