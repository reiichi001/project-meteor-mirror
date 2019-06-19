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

-- Dumping structure for table ffxiv_server.server_items_dealing
CREATE TABLE IF NOT EXISTS `server_items_dealing` (
  `id` bigint(20) unsigned NOT NULL,
  `dealingValue` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `dealingMode` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `dealingAttached1` int(11) DEFAULT '0',
  `dealingAttached2` int(11) NOT NULL DEFAULT '0',
  `dealingAttached3` int(11) NOT NULL DEFAULT '0',
  `dealingTag` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `bazaarMode` tinyint(3) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IF(@OLD_FOREIGN_KEY_CHECKS IS NULL, 1, @OLD_FOREIGN_KEY_CHECKS) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
