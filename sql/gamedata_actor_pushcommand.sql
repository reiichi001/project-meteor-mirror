/*
MySQL Data Transfer
Source Host: localhost
Source Database: ffxiv_server
Target Host: localhost
Target Database: ffxiv_server
Date: 1/10/2017 4:44:59 PM
*/

SET FOREIGN_KEY_CHECKS=0;
-- ----------------------------
-- Table structure for gamedata_actor_pushcommand
-- ----------------------------
CREATE TABLE `gamedata_actor_pushcommand` (
  `id` int(10) unsigned NOT NULL,
  `pushCommand` smallint(5) unsigned NOT NULL DEFAULT '0',
  `pushCommandSub` smallint(6) NOT NULL DEFAULT '0',
  `pushCommandPriority` tinyint(3) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;

-- ----------------------------
-- Records 
-- ----------------------------
