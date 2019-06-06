/*
MySQL Data Transfer
Source Host: localhost
Source Database: ffxiv_server
Target Host: localhost
Target Database: ffxiv_server
Date: 12/17/2017 3:37:50 PM
*/

SET FOREIGN_KEY_CHECKS=0;
-- ----------------------------
-- Table structure for characters_inventory_equipment
-- ----------------------------
CREATE TABLE `characters_inventory_equipment` (
  `characterId` int(10) unsigned NOT NULL,
  `classId` smallint(5) unsigned NOT NULL,
  `equipSlot` smallint(5) unsigned NOT NULL,
  `itemId` bigint(10) unsigned NOT NULL,
  PRIMARY KEY (`characterId`,`classId`,`equipSlot`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records 
-- ----------------------------
