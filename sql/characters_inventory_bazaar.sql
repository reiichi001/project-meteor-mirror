/*
MySQL Data Transfer
Source Host: localhost
Source Database: ffxiv_server
Target Host: localhost
Target Database: ffxiv_server
Date: 12/17/2017 3:37:44 PM
*/

SET FOREIGN_KEY_CHECKS=0;
-- ----------------------------
-- Table structure for characters_inventory_bazaar
-- ----------------------------
CREATE TABLE `characters_inventory_bazaar` (
  `id` int(10) unsigned zerofill NOT NULL AUTO_INCREMENT,
  `characterId` int(10) unsigned NOT NULL DEFAULT '0',
  `rewardId` bigint(10) unsigned DEFAULT NULL,
  `seekId` bigint(10) unsigned DEFAULT '0',
  `rewardAmount` int(11) DEFAULT '0',
  `seekAmount` int(11) DEFAULT '0',
  `bazaarMode` tinyint(4) unsigned NOT NULL DEFAULT '0',
  `sellPrice` int(11) DEFAULT '0',
  PRIMARY KEY (`id`,`characterId`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records 
-- ----------------------------
