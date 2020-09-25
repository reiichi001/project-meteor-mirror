/*
MySQL Data Transfer
Source Host: localhost
Source Database: ffxiv_server
Target Host: localhost
Target Database: ffxiv_server
Date: 4/2/2017 2:27:54 PM
*/

SET FOREIGN_KEY_CHECKS=0;
-- ----------------------------
-- Table structure for characters_quest_scenario
-- ----------------------------
CREATE TABLE `characters_quest_scenario` (
  `characterId` int(10) unsigned NOT NULL,
  `slot` smallint(5) unsigned NOT NULL,
  `questId` int(10) unsigned NOT NULL,
  `currentPhase` int(10) unsigned NOT NULL DEFAULT '0',
  `questData` longtext,
  `questFlags` int(10) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`characterId`,`slot`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
