/*
MySQL Data Transfer
Source Host: localhost
Source Database: ffxiv_server
Target Host: localhost
Target Database: ffxiv_server
Date: 12/17/2017 3:38:35 PM
*/

SET FOREIGN_KEY_CHECKS=0;
-- ----------------------------
-- Table structure for server_items
-- ----------------------------
CREATE TABLE `server_items` (
  `id` bigint(20) unsigned zerofill NOT NULL AUTO_INCREMENT,
  `itemId` int(10) unsigned NOT NULL,
  `quantity` int(10) unsigned DEFAULT '1',
  `quality` tinyint(3) unsigned DEFAULT '1',
  `isExclusive` tinyint(1) unsigned DEFAULT '0',
  `isAttached` tinyint(1) unsigned DEFAULT '0',
  `isDealing` tinyint(1) unsigned DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records 
-- ----------------------------
