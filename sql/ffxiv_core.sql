/*
MySQL Data Transfer
Source Host: localhost
Source Database: ffxiv_server
Target Host: localhost
Target Database: ffxiv_server
Date: 4/18/2016 1:39:44 PM
*/

SET FOREIGN_KEY_CHECKS=0;
-- ----------------------------
-- Table structure for characters
-- ----------------------------
CREATE TABLE `characters` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `userId` int(11) unsigned NOT NULL,
  `slot` smallint(6) unsigned NOT NULL,
  `serverId` int(11) NOT NULL,
  `name` varchar(32) NOT NULL,
  `state` smallint(5) unsigned NOT NULL DEFAULT '0',
  `creationDate` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `isLegacy` smallint(1) unsigned DEFAULT '0',
  `doRename` smallint(1) unsigned DEFAULT '0',
  `playTime` int(10) unsigned NOT NULL DEFAULT '0',
  `positionX` float NOT NULL DEFAULT '0',
  `positionY` float NOT NULL DEFAULT '0',
  `positionZ` float NOT NULL DEFAULT '0',
  `rotation` float NOT NULL DEFAULT '0',
  `actorState` smallint(5) unsigned DEFAULT '0',
  `currentZoneId` smallint(5) unsigned DEFAULT '0',
  `guardian` tinyint(3) unsigned NOT NULL,
  `birthDay` tinyint(3) unsigned NOT NULL,
  `birthMonth` tinyint(3) unsigned NOT NULL,
  `initialTown` tinyint(3) unsigned NOT NULL,
  `tribe` tinyint(3) unsigned NOT NULL,
  `gcCurrent` tinyint(3) unsigned DEFAULT '0',
  `gcLimsaRank` tinyint(3) unsigned DEFAULT '127',
  `gcGridaniaRank` tinyint(3) unsigned DEFAULT '127',
  `gcUldahRank` tinyint(3) unsigned DEFAULT '127',
  `currentTitle` int(10) unsigned DEFAULT '0',
  `currentParty` int(10) unsigned DEFAULT '0',
  `restBonus` int(10) DEFAULT '0',
  `achievementPoints` int(10) unsigned DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=158 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for characters_achievements
-- ----------------------------
CREATE TABLE `characters_achievements` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `characterId` int(10) unsigned NOT NULL,
  `achievementId` int(10) unsigned NOT NULL,
  `timeDone` int(10) unsigned DEFAULT NULL,
  `progress` int(10) unsigned DEFAULT '0',
  `progressFlags` int(10) unsigned DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=latin1;

-- ----------------------------
-- Table structure for characters_appearance
-- ----------------------------
CREATE TABLE `characters_appearance` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `characterId` int(10) unsigned NOT NULL,
  `baseId` int(10) unsigned NOT NULL,
  `size` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `voice` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `skinColor` smallint(5) unsigned NOT NULL,
  `hairStyle` smallint(5) unsigned NOT NULL,
  `hairColor` smallint(5) unsigned NOT NULL,
  `hairHighlightColor` smallint(5) unsigned NOT NULL DEFAULT '0',
  `hairVariation` smallint(5) unsigned NOT NULL,
  `eyeColor` smallint(5) unsigned NOT NULL,
  `faceType` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `faceEyebrows` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `faceEyeShape` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `faceIrisSize` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `faceNose` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `faceMouth` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `faceFeatures` tinyint(3) unsigned NOT NULL,
  `ears` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `characteristics` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `characteristicsColor` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `mainhand` int(10) unsigned NOT NULL,
  `offhand` int(10) unsigned NOT NULL,
  `head` int(10) unsigned NOT NULL,
  `body` int(10) unsigned NOT NULL,
  `hands` int(10) unsigned NOT NULL,
  `legs` int(10) unsigned NOT NULL,
  `feet` int(10) unsigned NOT NULL,
  `waist` int(10) unsigned NOT NULL,
  `leftFinger` int(10) unsigned NOT NULL,
  `rightFinger` int(10) unsigned NOT NULL,
  `leftEar` int(10) unsigned NOT NULL,
  `rightEar` int(10) unsigned NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=44 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for characters_blacklist
-- ----------------------------
CREATE TABLE `characters_blacklist` (
  `characterId` int(10) unsigned NOT NULL,
  `slot` int(10) unsigned NOT NULL,
  `name` varchar(255) NOT NULL,
  PRIMARY KEY (`characterId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for characters_chocobo
-- ----------------------------
CREATE TABLE `characters_chocobo` (
  `characterId` int(10) unsigned NOT NULL,
  `hasChocobo` tinyint(1) unsigned DEFAULT '0',
  `hasGoobbue` tinyint(1) unsigned DEFAULT '0',
  `chocoboAppearance` tinyint(3) unsigned DEFAULT NULL,
  `chocoboName` varchar(255) DEFAULT '',
  PRIMARY KEY (`characterId`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- ----------------------------
-- Table structure for characters_class_exp
-- ----------------------------
CREATE TABLE `characters_class_exp` (
  `characterId` int(10) unsigned NOT NULL,
  `pug` int(10) unsigned DEFAULT '0',
  `gla` int(10) unsigned DEFAULT '0',
  `mrd` int(10) unsigned DEFAULT '0',
  `arc` int(10) unsigned DEFAULT '0',
  `lnc` int(10) unsigned DEFAULT '0',
  `thm` int(10) unsigned DEFAULT '0',
  `cnj` int(10) unsigned DEFAULT '0',
  `crp` int(10) unsigned DEFAULT '0',
  `bsm` int(10) unsigned DEFAULT '0',
  `arm` int(10) unsigned DEFAULT '0',
  `gsm` int(10) unsigned DEFAULT '0',
  `ltw` int(10) unsigned DEFAULT '0',
  `wvr` int(10) unsigned DEFAULT '0',
  `alc` int(10) unsigned DEFAULT '0',
  `cul` int(10) unsigned DEFAULT '0',
  `min` int(10) unsigned DEFAULT '0',
  `btn` int(10) unsigned DEFAULT '0',
  `fsh` int(10) unsigned DEFAULT '0',
  PRIMARY KEY (`characterId`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- ----------------------------
-- Table structure for characters_class_levels
-- ----------------------------
CREATE TABLE `characters_class_levels` (
  `characterId` int(10) unsigned NOT NULL,
  `pug` smallint(6) DEFAULT '0',
  `gla` smallint(6) DEFAULT '0',
  `mrd` smallint(6) DEFAULT '0',
  `arc` smallint(6) DEFAULT '0',
  `lnc` smallint(6) DEFAULT '0',
  `thm` smallint(6) DEFAULT '0',
  `cnj` smallint(6) DEFAULT '0',
  `crp` smallint(6) DEFAULT '0',
  `bsm` smallint(6) DEFAULT '0',
  `arm` smallint(6) DEFAULT '0',
  `gsm` smallint(6) DEFAULT '0',
  `ltw` smallint(6) DEFAULT '0',
  `wvr` smallint(6) DEFAULT '0',
  `alc` smallint(6) DEFAULT '0',
  `cul` smallint(6) DEFAULT '0',
  `min` smallint(6) DEFAULT '0',
  `btn` smallint(6) DEFAULT '0',
  `fsh` smallint(6) DEFAULT '0',
  PRIMARY KEY (`characterId`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- ----------------------------
-- Table structure for characters_customattributes
-- ----------------------------
CREATE TABLE `characters_customattributes` (
  `characterId` int(10) unsigned NOT NULL,
  `pointsRemaining` int(11) DEFAULT '0',
  `strSpent` int(11) DEFAULT '0',
  `vitSpent` int(11) DEFAULT '0',
  `dexSpent` int(11) DEFAULT '0',
  `intSpent` int(11) DEFAULT '0',
  `minSpent` int(11) DEFAULT '0',
  `pieSpent` int(11) DEFAULT '0',
  PRIMARY KEY (`characterId`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- ----------------------------
-- Table structure for characters_friendlist
-- ----------------------------
CREATE TABLE `characters_friendlist` (
  `id` int(10) unsigned zerofill NOT NULL AUTO_INCREMENT,
  `characterId` int(10) unsigned NOT NULL,
  `slot` int(10) unsigned NOT NULL,
  `name` varchar(255) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for characters_hotbar
-- ----------------------------
CREATE TABLE `characters_hotbar` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `characterId` int(10) unsigned NOT NULL,
  `classId` smallint(5) unsigned NOT NULL,
  `hotbarSlot` smallint(5) unsigned NOT NULL,
  `commandId` int(10) unsigned NOT NULL,
  `recastTime` int(10) unsigned DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- ----------------------------
-- Table structure for characters_inventory
-- ----------------------------
CREATE TABLE `characters_inventory` (
  `id` int(10) unsigned zerofill NOT NULL AUTO_INCREMENT,
  `characterId` int(10) unsigned NOT NULL,
  `slot` int(10) unsigned NOT NULL,
  `inventoryType` smallint(5) unsigned NOT NULL DEFAULT '0',
  `serverItemId` int(10) unsigned NOT NULL,
  `quantity` int(10) unsigned NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=968 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for characters_inventory_equipment
-- ----------------------------
CREATE TABLE `characters_inventory_equipment` (
  `characterId` int(10) unsigned NOT NULL,
  `classId` smallint(5) unsigned NOT NULL,
  `equipSlot` smallint(5) unsigned NOT NULL,
  `itemId` bigint(10) unsigned NOT NULL,
  PRIMARY KEY (`characterId`,`classId`,`equipSlot`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for characters_linkshells
-- ----------------------------
CREATE TABLE `characters_linkshells` (
  `id` int(10) unsigned zerofill NOT NULL AUTO_INCREMENT,
  `characterId` int(10) unsigned NOT NULL,
  `slot` int(10) unsigned NOT NULL,
  `linkshellId` int(10) unsigned NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for characters_npclinkshell
-- ----------------------------
CREATE TABLE `characters_npclinkshell` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `characterId` int(11) unsigned NOT NULL,
  `npcLinkshellId` smallint(5) unsigned NOT NULL,
  `isCalling` tinyint(1) unsigned DEFAULT '0',
  `isExtra` tinyint(1) unsigned DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=latin1;

-- ----------------------------
-- Table structure for characters_parametersave
-- ----------------------------
CREATE TABLE `characters_parametersave` (
  `characterId` int(10) unsigned NOT NULL,
  `hp` smallint(6) NOT NULL,
  `hpMax` smallint(6) NOT NULL,
  `mp` smallint(6) NOT NULL,
  `mpMax` smallint(6) NOT NULL,
  `mainSkill` tinyint(3) unsigned NOT NULL,
  `mainSkillLevel` smallint(6) NOT NULL,
  PRIMARY KEY (`characterId`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- ----------------------------
-- Table structure for characters_quest_completed
-- ----------------------------
CREATE TABLE `characters_quest_completed` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `characterId` int(10) unsigned NOT NULL,
  `questId` int(10) unsigned NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- ----------------------------
-- Table structure for characters_quest_guildleve_local
-- ----------------------------
CREATE TABLE `characters_quest_guildleve_local` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `characterId` int(10) unsigned NOT NULL,
  `slot` smallint(5) unsigned NOT NULL,
  `questId` int(10) unsigned NOT NULL,
  `abandoned` tinyint(1) unsigned DEFAULT '0',
  `completed` tinyint(1) DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=latin1;

-- ----------------------------
-- Table structure for characters_quest_guildleve_regional
-- ----------------------------
CREATE TABLE `characters_quest_guildleve_regional` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `characterId` int(10) unsigned NOT NULL,
  `slot` smallint(5) unsigned NOT NULL,
  `guildleveId` smallint(3) unsigned NOT NULL,
  `abandoned` tinyint(1) unsigned DEFAULT '0',
  `completed` tinyint(1) DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=latin1;

-- ----------------------------
-- Table structure for characters_quest_guildlevehistory
-- ----------------------------
CREATE TABLE `characters_quest_guildlevehistory` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `characterId` int(10) unsigned NOT NULL,
  `questId` int(10) unsigned NOT NULL,
  `timeCompleted` timestamp NOT NULL DEFAULT '0000-00-00 00:00:00',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- ----------------------------
-- Table structure for characters_quest_scenario
-- ----------------------------
CREATE TABLE `characters_quest_scenario` (
  `characterId` int(10) unsigned NOT NULL,
  `slot` smallint(5) unsigned NOT NULL,
  `questId` int(10) unsigned NOT NULL,
  `questData` longtext,
  `questFlags` int(10) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`characterId`,`slot`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- ----------------------------
-- Table structure for characters_statuseffect
-- ----------------------------
CREATE TABLE `characters_statuseffect` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `characterId` int(10) unsigned NOT NULL,
  `statusId` tinyint(3) unsigned NOT NULL,
  `expireTime` int(10) unsigned NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- ----------------------------
-- Table structure for characters_timers
-- ----------------------------
CREATE TABLE `characters_timers` (
  `characterId` int(10) unsigned NOT NULL DEFAULT '0',
  `thousandmaws` int(10) unsigned DEFAULT '0',
  `dzemaeldarkhold` int(10) unsigned DEFAULT '0',
  `bowlofembers_hard` int(10) unsigned DEFAULT '0',
  `bowlofembers` int(10) unsigned DEFAULT '0',
  `thornmarch` int(10) unsigned DEFAULT '0',
  `aurumvale` int(10) unsigned DEFAULT '0',
  `cutterscry` int(10) unsigned DEFAULT '0',
  `battle_aleport` int(10) unsigned DEFAULT '0',
  `battle_hyrstmill` int(10) unsigned DEFAULT '0',
  `battle_goldenbazaar` int(10) unsigned DEFAULT '0',
  `howlingeye_hard` int(10) unsigned DEFAULT '0',
  `howlingeye` int(10) unsigned DEFAULT '0',
  `castrumnovum` int(10) unsigned DEFAULT '0',
  `bowlofembers_extreme` int(10) unsigned DEFAULT '0',
  `rivenroad` int(10) unsigned DEFAULT '0',
  `rivenroad_hard` int(10) unsigned DEFAULT '0',
  `behests` int(10) unsigned DEFAULT '0',
  `companybehests` int(10) unsigned DEFAULT '0',
  `returntimer` int(10) unsigned DEFAULT '0',
  `skirmish` int(10) unsigned DEFAULT '0',
  PRIMARY KEY (`characterId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for linkshells
-- ----------------------------
CREATE TABLE `linkshells` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `name` varchar(255) NOT NULL,
  `crestIcon` smallint(5) unsigned NOT NULL,
  `founder` int(10) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for parties
-- ----------------------------
CREATE TABLE `parties` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `leaderCharacterId` int(10) unsigned NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for reserved_names
-- ----------------------------
CREATE TABLE `reserved_names` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `userId` int(11) NOT NULL,
  `name` varchar(255) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for retainers
-- ----------------------------
CREATE TABLE `retainers` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `characterId` int(10) unsigned NOT NULL,
  `name` varchar(255) NOT NULL,
  `slot` smallint(5) unsigned NOT NULL,
  `doRename` smallint(1) unsigned NOT NULL DEFAULT '0',
  `locationId` smallint(5) unsigned NOT NULL,
  `state` tinyint(4) unsigned DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8;

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
) ENGINE=InnoDB AUTO_INCREMENT=985 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for server_sessions
-- ----------------------------
CREATE TABLE `server_sessions` (
  `id` char(255) NOT NULL,
  `characterId` int(11) NOT NULL,
  `actorId` int(10) unsigned NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- ----------------------------
-- Table structure for server_zones
-- ----------------------------
CREATE TABLE `server_zones` (
  `id` int(10) unsigned NOT NULL,
  `regionId` smallint(6) unsigned NOT NULL,
  `zoneName` varchar(255) DEFAULT NULL,
  `placeName` varchar(255) NOT NULL,
  `className` varchar(30) NOT NULL,
  `dayMusic` smallint(6) unsigned DEFAULT '0',
  `nightMusic` smallint(6) unsigned DEFAULT '0',
  `battleMusic` smallint(6) unsigned DEFAULT '0',
  `isIsolated` tinyint(1) DEFAULT '0',
  `isInn` tinyint(1) DEFAULT '0',
  `canRideChocobo` tinyint(1) DEFAULT '1',
  `canStealth` tinyint(1) DEFAULT '0',
  `isInstanceRaid` tinyint(1) unsigned DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- ----------------------------
-- Table structure for server_zones_privateareas
-- ----------------------------
CREATE TABLE `server_zones_privateareas` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `parentZoneId` int(10) unsigned NOT NULL,
  `privateAreaName` varchar(32) NOT NULL,
  `className` varchar(32) NOT NULL,
  `dayMusic` smallint(6) unsigned DEFAULT '0',
  `nightMusic` smallint(6) unsigned DEFAULT '0',
  `battleMusic` smallint(6) unsigned DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=latin1;

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
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=latin1;

-- ----------------------------
-- Table structure for servers
-- ----------------------------
CREATE TABLE `servers` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `name` varchar(32) NOT NULL,
  `address` varchar(255) NOT NULL,
  `port` smallint(6) unsigned NOT NULL,
  `listPosition` smallint(6) NOT NULL,
  `numchars` int(10) unsigned NOT NULL DEFAULT '0',
  `maxchars` int(10) unsigned NOT NULL DEFAULT '5000',
  `isActive` tinyint(1) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for sessions
-- ----------------------------
CREATE TABLE `sessions` (
  `id` char(56) NOT NULL,
  `userid` int(11) NOT NULL,
  `expiration` datetime NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `userid_UNIQUE` (`userid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for users
-- ----------------------------
CREATE TABLE `users` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(45) NOT NULL,
  `passhash` char(56) NOT NULL,
  `salt` char(56) NOT NULL,
  `email` varchar(256) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `name_UNIQUE` (`name`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records 
-- ----------------------------
INSERT INTO `server_zones` VALUES ('0', '0', null, '--', '', '0', '0', '0', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('128', '101', 'sea0Field01', 'Lower La Noscea', 'ZoneMasterSeaS0', '60', '60', '21', '0', '0', '1', '0', '0');
INSERT INTO `server_zones` VALUES ('129', '101', 'sea0Field02', 'Western La Noscea', 'ZoneMasterSeaS0', '60', '60', '21', '0', '0', '1', '0', '0');
INSERT INTO `server_zones` VALUES ('130', '101', 'sea0Field03', 'Eastern La Noscea', 'ZoneMasterSeaS0', '60', '60', '21', '0', '0', '1', '0', '0');
INSERT INTO `server_zones` VALUES ('131', '101', 'sea0Dungeon01', 'Mistbeard Cove', 'ZoneMasterSeaS0', '0', '0', '0', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('132', '101', 'sea0Dungeon02', 'Cassiopeia Hollow', 'ZoneMasterSeaS0', '0', '0', '0', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('133', '101', 'sea0Town01', 'Limsa Lominsa', 'ZoneMasterSeaS0', '59', '59', '0', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('134', '202', 'sea0Market01', 'Market Wards', 'ZoneMasterMarketSeaS0', '0', '0', '0', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('135', '101', 'sea0Field04', 'Upper La Noscea', 'ZoneMasterSeaS0', '60', '60', '21', '0', '0', '1', '0', '0');
INSERT INTO `server_zones` VALUES ('137', '101', null, 'U\'Ghamaro Mines', '', '0', '0', '0', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('138', '101', null, 'La Noscea', '', '60', '60', '21', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('139', '101', null, 'The Cieldalaes', '', '0', '0', '0', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('140', '101', null, 'Sailors Ward', '', '0', '0', '0', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('141', '101', 'sea0Field01a', 'Lower La Noscea', 'ZoneMasterSeaS0', '60', '60', '21', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('143', '102', 'roc0Field01', 'Coerthas Central Highlands', 'ZoneMasterRocR0', '55', '55', '15', '0', '0', '1', '0', '0');
INSERT INTO `server_zones` VALUES ('144', '102', 'roc0Field02', 'Coerthas Eastern Highlands', 'ZoneMasterRocR0', '55', '55', '15', '0', '0', '1', '0', '0');
INSERT INTO `server_zones` VALUES ('145', '102', 'roc0Field03', 'Coerthas Eastern Lowlands', 'ZoneMasterRocR0', '55', '55', '15', '0', '0', '1', '0', '0');
INSERT INTO `server_zones` VALUES ('146', '102', null, 'Coerthas', '', '55', '55', '15', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('147', '102', 'roc0Field04', 'Coerthas Central Lowlands', 'ZoneMasterRocR0', '55', '55', '15', '0', '0', '1', '0', '0');
INSERT INTO `server_zones` VALUES ('148', '102', 'roc0Field05', 'Coerthas Western Highlands', 'ZoneMasterRocR0', '55', '55', '15', '0', '0', '1', '0', '0');
INSERT INTO `server_zones` VALUES ('150', '103', 'fst0Field01', 'Central Shroud', 'ZoneMasterFstF0', '52', '52', '13', '0', '0', '1', '0', '0');
INSERT INTO `server_zones` VALUES ('151', '103', 'fst0Field02', 'East Shroud', 'ZoneMasterFstF0', '52', '52', '13', '0', '0', '1', '0', '0');
INSERT INTO `server_zones` VALUES ('152', '103', 'fst0Field03', 'North Shroud', 'ZoneMasterFstF0', '52', '52', '13', '0', '0', '1', '0', '0');
INSERT INTO `server_zones` VALUES ('153', '103', 'fst0Field04', 'West Shroud', 'ZoneMasterFstF0', '52', '52', '13', '0', '0', '1', '0', '0');
INSERT INTO `server_zones` VALUES ('154', '103', 'fst0Field05', 'South Shroud', 'ZoneMasterFstF0', '52', '52', '13', '0', '0', '1', '0', '0');
INSERT INTO `server_zones` VALUES ('155', '103', 'fst0Town01', 'Gridania', 'ZoneMasterFstF0', '51', '51', '0', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('156', '103', null, 'The Black Shroud', '', '0', '0', '0', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('157', '103', 'fst0Dungeon01', 'The Mun-Tuy Cellars', 'ZoneMasterFstF0', '0', '0', '0', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('158', '103', 'fst0Dungeon02', 'The Tam-Tara Deepcroft', 'ZoneMasterFstF0', '0', '0', '0', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('159', '103', 'fst0Dungeon03', 'The Thousand Maws of Toto-Rak', 'ZoneMasterFstF0', '0', '0', '0', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('160', '204', 'fst0Market01', 'Market Wards', 'ZoneMasterMarketFstF0', '0', '0', '0', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('161', '103', null, 'Peasants Ward', '', '0', '0', '0', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('162', '103', null, 'Central Shroud', '', '52', '52', '13', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('164', '106', 'fst0Battle01', 'Central Shroud', 'ZoneMasterBattleFstF0', '0', '0', '13', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('165', '106', 'fst0Battle02', 'Central Shroud', 'ZoneMasterBattleFstF0', '0', '0', '13', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('166', '106', 'fst0Battle03', 'Central Shroud', 'ZoneMasterBattleFstF0', '0', '0', '13', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('167', '106', 'fst0Battle04', 'Central Shroud', 'ZoneMasterBattleFstF0', '0', '0', '13', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('168', '106', 'fst0Battle05', 'Central Shroud', 'ZoneMasterBattleFstF0', '0', '0', '13', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('170', '104', 'wil0Field01', 'Central Thanalan', 'ZoneMasterWilW0', '68', '68', '25', '0', '0', '1', '0', '0');
INSERT INTO `server_zones` VALUES ('171', '104', 'wil0Field02', 'Eastern Thanalan', 'ZoneMasterWilW0', '68', '68', '25', '0', '0', '1', '0', '0');
INSERT INTO `server_zones` VALUES ('172', '104', 'wil0Field03', 'Western Thanalan', 'ZoneMasterWilW0', '68', '68', '25', '0', '0', '1', '0', '0');
INSERT INTO `server_zones` VALUES ('173', '104', 'wil0Field04', 'Northern Thanalan', 'ZoneMasterWilW0', '68', '68', '25', '0', '0', '1', '0', '0');
INSERT INTO `server_zones` VALUES ('174', '104', 'wil0Field05', 'Southern Thanalan', 'ZoneMasterWilW0', '68', '68', '25', '0', '0', '1', '0', '0');
INSERT INTO `server_zones` VALUES ('175', '104', 'wil0Town01', 'Ul\'dah', 'ZoneMasterWilW0', '66', '66', '0', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('176', '104', 'wil0Dungeon01', 'Nanawa Mines', 'ZoneMasterWilW0', '0', '0', '0', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('177', '207', '_jail', '-', 'ZoneMasterJail', '0', '0', '0', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('178', '104', 'wil0Dungeon02', 'Copperbell Mines', 'ZoneMasterWilW0', '0', '0', '0', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('179', '104', null, 'Thanalan', '', '0', '0', '0', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('180', '205', 'wil0Market01', 'Market Wards', 'ZoneMasterMarketWilW0', '0', '0', '0', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('181', '104', null, 'Merchants Ward', '', '0', '0', '0', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('182', '104', null, 'Central Thanalan', '', '68', '68', '25', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('184', '107', 'wil0Battle01', 'Ul\'dah', 'ZoneMasterBattleWilW0', '0', '0', '0', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('185', '107', 'wil0Battle01', 'Ul\'dah', 'ZoneMasterBattleWilW0', '0', '0', '0', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('186', '104', 'wil0Battle02', 'Ul\'dah', 'ZoneMasterBattleWilW0', '0', '0', '0', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('187', '104', 'wil0Battle03', 'Ul\'dah', 'ZoneMasterBattleWilW0', '0', '0', '0', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('188', '104', 'wil0Battle04', 'Ul\'dah', 'ZoneMasterBattleWilW0', '0', '0', '0', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('190', '105', 'lak0Field01', 'Mor Dhona', 'ZoneMasterLakL0', '49', '49', '11', '0', '0', '1', '0', '0');
INSERT INTO `server_zones` VALUES ('192', '111', 'ocn0Battle01', 'Rhotano Sea', 'ZoneMasterBattleOcnO0', '0', '0', '0', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('193', '111', 'ocn0Battle02', 'Rhotano Sea', 'ZoneMasterBattleOcnO0', '7', '0', '0', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('194', '111', 'ocn0Battle03', 'Rhotano Sea', 'ZoneMasterBattleOcnO0', '0', '0', '0', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('195', '111', 'ocn0Battle04', 'Rhotano Sea', 'ZoneMasterBattleOcnO0', '0', '0', '0', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('196', '111', 'ocn0Battle05', 'Rhotano Sea', 'ZoneMasterBattleOcnO0', '0', '0', '0', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('198', '111', 'ocn0Battle06', 'Rhotano Sea', 'ZoneMasterBattleOcnO0', '0', '0', '0', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('200', '111', null, 'Strait of Merlthor', '', '0', '0', '0', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('201', '111', null, '-', '', '0', '0', '0', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('204', '101', 'sea0Field02a', 'Western La Noscea', '', '60', '60', '21', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('205', '101', 'sea0Field03a', 'Eastern La Noscea', '', '60', '60', '21', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('206', '103', 'fst0Town1a', 'Gridania', 'ZoneMasterFstF0', '52', '52', '13', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('207', '103', null, 'North Shroud', '', '52', '52', '13', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('208', '103', null, 'South Shroud', '', '52', '52', '13', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('209', '104', 'wil0Town01a', 'Ul\'dah', 'ZoneMasterWilW0', '66', '66', '0', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('210', '104', null, 'Eastern Thanalan', '', '68', '68', '25', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('211', '104', null, 'Western Thanalan', '', '68', '68', '25', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('230', '101', 'sea0Town01a', 'Limsa Lominsa', 'ZoneMasterSeaS0', '59', '59', '0', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('231', '102', 'roc0Dungeon01', 'Dzemael Darkhold', '', '0', '0', '0', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('232', '101', 'sea0Office01', 'Maelstrom Command', '', '0', '0', '0', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('233', '104', 'wil0Office01', 'Hall of Flames', '', '0', '0', '0', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('234', '103', 'fst0Office01', 'Adders\' Nest', '', '0', '0', '0', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('235', '101', null, 'Shposhae', '', '0', '0', '0', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('236', '101', null, 'Locke\'s Lie', '', '0', '0', '0', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('237', '101', null, 'Turtleback Island', '', '0', '0', '0', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('238', '103', 'fst0Field04', 'Thornmarch', '', '0', '0', '0', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('239', '102', null, 'The Howling Eye', '', '0', '0', '0', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('240', '104', 'wil0Field05a', 'The Bowl of Embers', '', '0', '0', '0', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('244', '209', 'prv0Inn01', 'Inn Room', 'ZoneMasterPrvI0', '61', '61', '0', '0', '1', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('245', '102', 'roc0Dungeon02', 'The Aurum Vale', '', '0', '0', '0', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('246', '104', null, 'Cutter\'s Cry', '', '0', '0', '0', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('247', '103', null, 'North Shroud', '', '0', '0', '0', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('248', '101', null, 'Western La Noscea', '', '0', '0', '0', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('249', '104', null, 'Eastern Thanalan', '', '0', '0', '0', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('250', '102', null, 'The Howling Eye', '', '0', '0', '0', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('251', '105', null, 'Transmission Tower', '', '0', '0', '0', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('252', '102', null, 'The Aurum Vale', '', '0', '0', '0', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('253', '102', null, 'The Aurum Vale', '', '0', '0', '0', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('254', '104', null, 'Cutter\'s Cry', '', '0', '0', '0', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('255', '104', null, 'Cutter\'s Cry', '', '0', '0', '0', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('256', '102', null, 'The Howling Eye', '', '0', '0', '0', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('257', '109', null, 'Rivenroad', '', '0', '0', '0', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('258', '103', null, 'North Shroud', '', '0', '0', '0', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('259', '103', null, 'North Shroud', '', '0', '0', '0', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('260', '101', null, 'Western La Noscea', '', '0', '0', '0', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('261', '101', null, 'Western La Noscea', '', '0', '0', '0', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('262', '104', null, 'Eastern Thanalan', '', '0', '0', '0', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('263', '104', null, 'Eastern Thanalan', '', '0', '0', '0', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('264', '105', 'lak0Field01', 'Transmission Tower', '', '0', '0', '0', '0', '0', '1', '0', '0');
INSERT INTO `server_zones` VALUES ('265', '104', null, 'The Bowl of Embers', '', '0', '0', '0', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('266', '105', 'lak0Field01a', 'Mor Dhona', 'ZoneMasterLakL0', '49', '49', '11', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('267', '109', null, 'Rivenroad', '', '0', '0', '0', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('268', '109', null, 'Rivenroad', '', '0', '0', '0', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('269', '101', null, 'Locke\'s Lie', '', '0', '0', '0', '0', '0', '0', '0', '0');
INSERT INTO `server_zones` VALUES ('270', '101', null, 'Turtleback Island', '', '0', '0', '0', '0', '0', '0', '0', '0');
INSERT INTO `server_zones_privateareas` VALUES ('1', '175', 'PrivateAreaMasterPast', 'PrivateAreaMasterPast', '0', '0', '0');
INSERT INTO `server_zones_privateareas` VALUES ('2', '230', 'PrivateAreaMasterPast', 'PrivateAreaMasterPast', '0', '0', '0');
INSERT INTO `server_zones_privateareas` VALUES ('3', '193', 'ContentSimpleContent30002', 'PrivateAreaMasterSimpleContent', '0', '0', '0');
INSERT INTO `server_zones_spawnlocations` VALUES ('1', '155', null, '2', '58.92', '4', '-1219.07', '0.52');
INSERT INTO `server_zones_spawnlocations` VALUES ('2', '133', null, '2', '-444.266', '39.518', '191', '1.9');
INSERT INTO `server_zones_spawnlocations` VALUES ('3', '175', null, '2', '-110.157', '202', '171.345', '0');
INSERT INTO `server_zones_spawnlocations` VALUES ('4', '193', null, '2', '0.016', '10.35', '-36.91', '0.025');
INSERT INTO `server_zones_spawnlocations` VALUES ('5', '166', null, '2', '356.09', '3.74', '-701.62', '-1.4');
INSERT INTO `server_zones_spawnlocations` VALUES ('6', '175', 'PrivateAreaMasterPast', '2', '12.63', '196.05', '131.01', '-1.34');
INSERT INTO `server_zones_spawnlocations` VALUES ('7', '128', null, '2', '-8.48', '45.36', '139.5', '2.02');
INSERT INTO `server_zones_spawnlocations` VALUES ('8', '230', 'PrivateAreaMasterPast', '0', '-838.1', '6', '231.94', '1.1');
INSERT INTO `server_zones_spawnlocations` VALUES ('9', '193', null, '16', '-5', '16.35', '6', '0.5');
INSERT INTO `server_zones_spawnlocations` VALUES ('10', '166', null, '16', '356.09', '3.74', '-701.62', '-1.4');
INSERT INTO `servers` VALUES ('1', 'Fernehalwes', '127.0.0.1', '54992', '1', '1', '5000', '1');