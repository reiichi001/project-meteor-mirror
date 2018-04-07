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
  `id` int(10) unsigned zerofill NOT NULL AUTO_INCREMENT,
  `retainerId` int(10) unsigned NOT NULL,
  `itemPackage` smallint(5) unsigned NOT NULL DEFAULT '0',
  `serverItemId` int(10) unsigned NOT NULL,
  PRIMARY KEY (`id`,`retainerId`)
) ENGINE=InnoDB AUTO_INCREMENT=345 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records 
-- ----------------------------
