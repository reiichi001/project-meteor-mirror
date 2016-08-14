/*
MySQL Data Transfer
Source Host: localhost
Source Database: ffxiv_server
Target Host: localhost
Target Database: ffxiv_server
Date: 8/14/2016 9:43:11 AM
*/

SET FOREIGN_KEY_CHECKS=0;
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
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=latin1;

-- ----------------------------
-- Records 
-- ----------------------------
INSERT INTO `server_seamless_zonechange_bounds` VALUES ('1', '103', '155', '206', '115', '-1219', '55', '-1217', '33', '95', '-1279', '-1261', '55', '-1219', '95', '-1261');
INSERT INTO `server_seamless_zonechange_bounds` VALUES ('2', '103', '155', '150', '255', '-1139', '304', '-1125', '304', '338', '-1066', '-1046', '255', '-1125', '338', '-1066');
