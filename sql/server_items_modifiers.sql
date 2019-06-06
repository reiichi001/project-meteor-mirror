-- --------------------------------------------------------
-- Host:                         127.0.0.1
-- Server version:               5.6.17 - MySQL Community Server (GPL)
-- Server OS:                    Win64
-- HeidiSQL Version:             10.1.0.5464
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;

-- Dumping structure for table ffxiv_server.server_items_modifiers
CREATE TABLE IF NOT EXISTS `server_items_modifiers` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `durability` mediumint(8) unsigned NOT NULL DEFAULT '0',
  `mainQuality` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `subQuality1` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `subQuality2` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `subQuality3` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `param1` mediumint(8) unsigned NOT NULL DEFAULT '0',
  `param2` mediumint(8) unsigned NOT NULL DEFAULT '0',
  `param3` mediumint(8) unsigned NOT NULL DEFAULT '0',
  `spiritbind` smallint(5) unsigned NOT NULL DEFAULT '0',
  `materia1` smallint(5) unsigned NOT NULL DEFAULT '0',
  `materia2` smallint(5) unsigned NOT NULL DEFAULT '0',
  `materia3` smallint(5) unsigned NOT NULL DEFAULT '0',
  `materia4` smallint(5) unsigned NOT NULL DEFAULT '0',
  `materia5` smallint(5) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=142 DEFAULT CHARSET=latin1;

/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IF(@OLD_FOREIGN_KEY_CHECKS IS NULL, 1, @OLD_FOREIGN_KEY_CHECKS) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
