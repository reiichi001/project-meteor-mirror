/*
MySQL Data Transfer
Source Host: localhost
Source Database: ffxiv_server
Target Host: localhost
Target Database: ffxiv_server
Date: 7/9/2017 11:38:35 AM
*/

SET FOREIGN_KEY_CHECKS=0;
-- ----------------------------
-- Table structure for gamedata_items_graphics_extra
-- ----------------------------
CREATE TABLE `gamedata_items_graphics_extra` (
  `catalogID` int(10) unsigned NOT NULL,
  `offHandWeaponId` int(10) unsigned NOT NULL DEFAULT '0',
  `offHandEquipmentId` int(10) unsigned NOT NULL DEFAULT '0',
  `offHandVarientId` int(10) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`catalogID`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- ----------------------------
-- Records 
-- ----------------------------
INSERT INTO `gamedata_items_graphics_extra` VALUES ('4020001', '58', '1', '0');
INSERT INTO `gamedata_items_graphics_extra` VALUES ('4070001', '226', '1', '0');
