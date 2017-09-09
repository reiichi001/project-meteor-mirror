/*
MySQL Data Transfer
Source Host: localhost
Source Database: ffxiv_server
Target Host: localhost
Target Database: ffxiv_server
Date: 9/9/2017 2:31:34 PM
*/

SET FOREIGN_KEY_CHECKS=0;
-- ----------------------------
-- Table structure for server_items
-- ----------------------------
CREATE TABLE `server_items` (
  `id` int(10) unsigned zerofill NOT NULL AUTO_INCREMENT,
  `itemId` int(10) unsigned NOT NULL,
  `quality` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `itemType` tinyint(6) unsigned NOT NULL DEFAULT '0',
  `durability` int(11) NOT NULL DEFAULT '0',
  `spiritbind` smallint(5) unsigned DEFAULT '0',
  `materia1` tinyint(3) unsigned DEFAULT '0',
  `materia2` tinyint(3) unsigned DEFAULT '0',
  `materia3` tinyint(3) unsigned DEFAULT '0',
  `materia4` tinyint(3) unsigned DEFAULT '0',
  `materia5` tinyint(3) unsigned DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=1435 DEFAULT CHARSET=utf8;
