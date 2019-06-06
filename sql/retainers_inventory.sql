/*
MySQL Data Transfer
Source Host: localhost
Source Database: ffxiv_server
Target Host: localhost
Target Database: ffxiv_server
Date: 12/17/2017 3:38:25 PM
*/

SET FOREIGN_KEY_CHECKS=0;
-- ----------------------------
-- Table structure for retainers_inventory
-- ----------------------------
CREATE TABLE `retainers_inventory` (
  `retainerId` int(10) unsigned NOT NULL,
  `serverItemId` int(10) unsigned NOT NULL,
  `itemPackage` mediumint(8) unsigned NOT NULL,
  PRIMARY KEY (`retainerId`,`serverItemId`)
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records 
-- ----------------------------
