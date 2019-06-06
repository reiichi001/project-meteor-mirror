/*
MySQL Data Transfer
Source Host: localhost
Source Database: ffxiv_server
Target Host: localhost
Target Database: ffxiv_server
Date: 12/17/2017 3:37:39 PM
*/

SET FOREIGN_KEY_CHECKS=0;
-- ----------------------------
-- Table structure for characters_inventory
-- ----------------------------
CREATE TABLE `characters_inventory` (
  `characterId` int(10) unsigned NOT NULL,
  `serverItemId` int(10) unsigned NOT NULL,
  `itemPackage` smallint(5) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`characterId`,`serverItemId`,`itemPackage`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records 
-- ----------------------------
