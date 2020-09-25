/*
MySQL Data Transfer
Source Host: localhost
Source Database: ffxiv_server
Target Host: localhost
Target Database: ffxiv_server
Date: 3/14/2020 1:00:50 PM
*/

SET FOREIGN_KEY_CHECKS=0;
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
  `neck` int(10) unsigned NOT NULL DEFAULT '0',
  `leftIndex` int(10) unsigned NOT NULL DEFAULT '0',
  `rightIndex` int(10) unsigned NOT NULL DEFAULT '0',
  `leftFinger` int(10) unsigned NOT NULL DEFAULT '0',
  `rightFinger` int(10) unsigned NOT NULL DEFAULT '0',
  `leftEar` int(10) unsigned NOT NULL DEFAULT '0',
  `rightEar` int(10) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8;