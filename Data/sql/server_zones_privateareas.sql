/*
MySQL Data Transfer
Source Host: localhost
Source Database: ffxiv_server
Target Host: localhost
Target Database: ffxiv_server
Date: 9/9/2017 4:27:43 PM
*/

SET FOREIGN_KEY_CHECKS=0;
-- ----------------------------
-- Table structure for server_zones_privateareas
-- ----------------------------
CREATE TABLE `server_zones_privateareas` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `parentZoneId` int(10) unsigned NOT NULL,
  `className` varchar(64) NOT NULL,
  `privateAreaName` varchar(32) NOT NULL,
  `privateAreaType` int(10) unsigned NOT NULL,
  `dayMusic` smallint(6) unsigned DEFAULT '0',
  `nightMusic` smallint(6) unsigned DEFAULT '0',
  `battleMusic` smallint(6) unsigned DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=latin1;

-- ----------------------------
-- Records 
-- ----------------------------
INSERT INTO `server_zones_privateareas` VALUES ('1', '184', '/Area/PrivateArea/PrivateAreaMasterPast', 'PrivateAreaMasterPast', '1', '66', '0', '0');
INSERT INTO `server_zones_privateareas` VALUES ('2', '230', '/Area/PrivateArea/PrivateAreaMasterPast', 'PrivateAreaMasterPast', '1', '59', '0', '0');
INSERT INTO `server_zones_privateareas` VALUES ('4', '133', '/Area/PrivateArea/PrivateAreaMasterPast', 'PrivateAreaMasterPast', '2', '40', '0', '0');
INSERT INTO `server_zones_privateareas` VALUES ('5', '155', '/Area/PrivateArea/PrivateAreaMasterPast', 'PrivateAreaMasterPast', '1', '51', '0', '0');
INSERT INTO `server_zones_privateareas` VALUES ('6', '155', '/Area/PrivateArea/PrivateAreaMasterPast', 'PrivateAreaMasterPast', '2', '40', '0', '0');
INSERT INTO `server_zones_privateareas` VALUES ('8', '175', '/Area/PrivateArea/PrivateAreaMasterPast', 'PrivateAreaMasterPast', '3', '66', '0', '0');
INSERT INTO `server_zones_privateareas` VALUES ('9', '175', '/Area/PrivateArea/PrivateAreaMasterPast', 'PrivateAreaMasterPast', '4', '40', '0', '0');
INSERT INTO `server_zones_privateareas` VALUES ('10', '180', '/Area/PrivateArea/PrivateAreaMasterBranch', 'PrivateAreaMasterMarket', '102', '48', '48', '48');
