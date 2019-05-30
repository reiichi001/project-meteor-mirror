/*
MySQL Data Transfer
Source Host: localhost
Source Database: ffxiv_server
Target Host: localhost
Target Database: ffxiv_server
Date: 9/9/2017 2:30:57 PM
*/

SET FOREIGN_KEY_CHECKS=0;
-- ----------------------------
-- Table structure for characters_retainers
-- ----------------------------
CREATE TABLE `characters_retainers` (
  `characterId` int(10) unsigned NOT NULL,
  `retainerId` int(10) unsigned NOT NULL,
  `doRename` tinyint(1) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`characterId`,`retainerId`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
