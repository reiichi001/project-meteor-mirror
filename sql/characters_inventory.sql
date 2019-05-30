/*
MySQL Data Transfer
Source Host: localhost
Source Database: ffxiv_server
Target Host: localhost
Target Database: ffxiv_server
Date: 9/9/2017 2:31:15 PM
*/

SET FOREIGN_KEY_CHECKS=0;
-- ----------------------------
-- Table structure for characters_inventory
-- ----------------------------
CREATE TABLE `characters_inventory` (
  `id` int(10) unsigned zerofill NOT NULL AUTO_INCREMENT,
  `characterId` int(10) unsigned NOT NULL,
  `inventoryType` smallint(5) unsigned NOT NULL DEFAULT '0',
  `serverItemId` int(10) unsigned NOT NULL,
  `quantity` int(10) unsigned NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`,`characterId`)
) ENGINE=InnoDB AUTO_INCREMENT=325 DEFAULT CHARSET=utf8;
