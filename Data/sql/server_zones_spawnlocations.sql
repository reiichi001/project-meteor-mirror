/*
MySQL Data Transfer
Source Host: localhost
Source Database: ffxiv_server
Target Host: localhost
Target Database: ffxiv_server
Date: 3/7/2017 8:30:07 AM
*/

SET FOREIGN_KEY_CHECKS=0;
-- ----------------------------
-- Table structure for server_zones_spawnlocations
-- ----------------------------
CREATE TABLE `server_zones_spawnlocations` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `zoneId` int(10) unsigned NOT NULL,
  `privateAreaName` varchar(32) DEFAULT NULL,
  `spawnType` tinyint(3) unsigned DEFAULT '0',
  `spawnX` float NOT NULL,
  `spawnY` float NOT NULL,
  `spawnZ` float NOT NULL,
  `spawnRotation` float NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=20 DEFAULT CHARSET=latin1;

-- ----------------------------
-- Records 
-- ----------------------------
INSERT INTO `server_zones_spawnlocations` VALUES ('1', '155', null, '15', '58.92', '4', '-1219.07', '0.52');
INSERT INTO `server_zones_spawnlocations` VALUES ('2', '133', null, '15', '-444.266', '39.518', '191', '1.9');
INSERT INTO `server_zones_spawnlocations` VALUES ('3', '175', null, '15', '-110.157', '202', '171.345', '0');
INSERT INTO `server_zones_spawnlocations` VALUES ('4', '193', null, '15', '0.016', '10.35', '-36.91', '0.025');
INSERT INTO `server_zones_spawnlocations` VALUES ('5', '166', null, '15', '356.09', '3.74', '-701.62', '-1.4');
INSERT INTO `server_zones_spawnlocations` VALUES ('6', '184', null, '15', '5.36433', '196', '133.656', '-2.84938');
INSERT INTO `server_zones_spawnlocations` VALUES ('7', '128', null, '15', '-8.48', '45.36', '139.5', '2.02');
INSERT INTO `server_zones_spawnlocations` VALUES ('8', '230', 'PrivateAreaMasterPast', '15', '-838.1', '6', '231.94', '1.1');
INSERT INTO `server_zones_spawnlocations` VALUES ('9', '193', null, '16', '-5', '16.35', '6', '0.5');
INSERT INTO `server_zones_spawnlocations` VALUES ('10', '166', null, '16', '356.09', '3.74', '-701.62', '-1.4');
INSERT INTO `server_zones_spawnlocations` VALUES ('11', '244', null, '15', '0.048', '0', '-5.737', '0');
INSERT INTO `server_zones_spawnlocations` VALUES ('12', '244', null, '15', '-160.048', '0', '-165.737', '0');
INSERT INTO `server_zones_spawnlocations` VALUES ('13', '244', null, '15', '160.048', '0', '154.263', '0');
INSERT INTO `server_zones_spawnlocations` VALUES ('14', '150', null, '15', '333.271', '5.889', '-943.275', '0.794');
INSERT INTO `server_zones_spawnlocations` VALUES ('15', '133', null, '15', '-8.062', '45.429', '139.364', '2.955');
INSERT INTO `server_zones_spawnlocations` VALUES ('16', '170', null, '15', '-27.015', '181.798', '-79.72', '2.513');
INSERT INTO `server_zones_spawnlocations` VALUES ('17', '184', null, '16', '-24.34', '192', '34.22', '0.78');
INSERT INTO `server_zones_spawnlocations` VALUES ('18', '184', null, '15', '-24.34', '192', '34.22', '0.78');
INSERT INTO `server_zones_spawnlocations` VALUES ('19', '184', null, '15', '-22', '196', '87', '1.8');
