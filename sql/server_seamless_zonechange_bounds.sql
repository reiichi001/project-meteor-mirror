/*
MySQL Data Transfer
Source Host: localhost
Source Database: ffxiv_server
Target Host: localhost
Target Database: ffxiv_server
Date: 8/14/2016 9:43:11 AM
*/

SET FOREIGN_KEY_CHECKS = 0;
SET autocommit = 0;

-- ----------------------------
-- Table structure for server_seamless_zonechange_bounds
-- ----------------------------
CREATE TABLE `server_seamless_zonechange_bounds` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `regionId` int(10) unsigned NOT NULL,
  `zoneId1` int(10) unsigned NOT NULL,
  `zoneId2` int(10) unsigned NOT NULL,
  `zone1_boundingbox_x1` float NOT NULL,
  `zone1_boundingbox_y1` float NOT NULL,
  `zone1_boundingbox_x2` float NOT NULL,
  `zone1_boundingbox_y2` float NOT NULL,
  `zone2_boundingbox_x1` float NOT NULL,
  `zone2_boundingbox_x2` float NOT NULL,
  `zone2_boundingbox_y1` float NOT NULL,
  `zone2_boundingbox_y2` float NOT NULL,
  `merge_boundingbox_x1` float NOT NULL,
  `merge_boundingbox_y1` float NOT NULL,
  `merge_boundingbox_x2` float NOT NULL,
  `merge_boundingbox_y2` float NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=14 DEFAULT CHARSET=latin1;

-- ----------------------------
-- Records 
-- ----------------------------
INSERT INTO `server_seamless_zonechange_bounds` VALUES ('1', '103', '155', '206', '115', '-1219', '55', '-1217', '33', '95', '-1279', '-1261', '55', '-1219', '95', '-1261');
INSERT INTO `server_seamless_zonechange_bounds` VALUES ('2', '103', '155', '150', '255', '-1139', '304', '-1125', '304', '338', '-1066', '-1046', '255', '-1125', '338', '-1066');
INSERT INTO `server_seamless_zonechange_bounds` VALUES ('3', '101', '133', '230', '-457', '131', '-436', '142', '-460', '-439', '92', '100', '-454', '101', '-439', '128');
INSERT INTO `server_seamless_zonechange_bounds` VALUES ('4', '101', '133', '230', '-486', '228', '-501', '218', '-482', '-503', '255', '242', '-490', '238', '-501', '229');
INSERT INTO `server_seamless_zonechange_bounds` VALUES ('5', '101', '133', '128', '-85', '165', '-79', '185', '-51', '-47', '149', '167', '-71', '160', '-69', '174');
INSERT INTO `server_seamless_zonechange_bounds` VALUES ('6', '101', '133', '230', '-483', '200', '-496', '181', '-506', '-514', '206', '177', '-500', '198', '-505', '185');
INSERT INTO `server_seamless_zonechange_bounds` VALUES ('7', '104', '170', '209', '87', '178', '110', '189', '89', '108', '142', '150', '94', '158', '108', '167');
INSERT INTO `server_seamless_zonechange_bounds` VALUES ('8', '104', '175', '209', '-134', '84', '-95', '92', '-120', '-82', '139', '143', '-120', '125', '-96', '124');
INSERT INTO `server_seamless_zonechange_bounds` VALUES ('9', '104', '170', '175', '-70', '-47', '-47', '-17', '-117', '-108', '-43', '-28', '-99', '-43', '-86', '-28');
INSERT INTO `server_seamless_zonechange_bounds` VALUES ('10', '104', '170', '175', '-39', '-33', '-24', '-9', '22', '23', '-7', '22', '-7', '-26', '-1', '-4');
INSERT INTO `server_seamless_zonechange_bounds` VALUES ('11', '104', '175', '209', '-243', '82', '-208', '107', '-264', '-230', '138', '173', '-254', '109', '-220', '128');
INSERT INTO `server_seamless_zonechange_bounds` VALUES ('12', '104', '175', '209', '0', '173', '24', '179', '-23', '9', '204', '232', '-6', '185', '13', '201');
INSERT INTO `server_seamless_zonechange_bounds` VALUES ('13', '104', '175', '209', '-20', '99', '5', '119', '-57', '-31', '124', '145', '-41', '115', '-15', '127');

COMMIT;