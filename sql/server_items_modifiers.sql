/*
MySQL Data Transfer
Source Host: localhost
Source Database: ffxiv_server
Target Host: localhost
Target Database: ffxiv_server
Date: 12/17/2017 3:38:44 PM
*/

SET FOREIGN_KEY_CHECKS=0;
-- ----------------------------
-- Table structure for server_items_modifiers
-- ----------------------------
CREATE TABLE `server_items_modifiers` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `durability` int(10) unsigned NOT NULL DEFAULT '0',
  `mainQuality` tinyint(3) unsigned NOT NULL DEFAULT '1',
  `subQuality1` tinyint(3) unsigned NOT NULL DEFAULT '1',
  `subQuality2` tinyint(3) unsigned NOT NULL DEFAULT '1',
  `subQuality3` tinyint(3) unsigned NOT NULL DEFAULT '1',
  `param1` int(10) unsigned NOT NULL DEFAULT '0',
  `param2` int(10) unsigned NOT NULL DEFAULT '0',
  `param3` int(10) unsigned NOT NULL DEFAULT '0',
  `spiritbind` smallint(5) unsigned NOT NULL DEFAULT '0',
  `materia1` smallint(5) unsigned NOT NULL DEFAULT '0',
  `materia2` smallint(5) unsigned NOT NULL DEFAULT '0',
  `materia3` smallint(5) unsigned NOT NULL DEFAULT '0',
  `materia4` smallint(5) unsigned NOT NULL DEFAULT '0',
  `materia5` smallint(5) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=2315 DEFAULT CHARSET=latin1;

-- ----------------------------
-- Records 
-- ----------------------------
