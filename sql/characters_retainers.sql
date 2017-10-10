/*
MySQL Data Transfer
Source Host: localhost
Source Database: ffxiv_server
Target Host: localhost
Target Database: ffxiv_server
Date: 4/15/2017 4:38:42 PM
*/

SET FOREIGN_KEY_CHECKS=0;
-- ----------------------------
-- Table structure for characters_retainers
-- ----------------------------
CREATE TABLE `characters_retainers` (
  `characterId` int(10) unsigned NOT NULL,
  `retainerId` int(10) unsigned NOT NULL,
  PRIMARY KEY (`characterId`,`retainerId`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- ----------------------------
-- Records 
-- ----------------------------
