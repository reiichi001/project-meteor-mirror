/*
MySQL Data Transfer
Source Host: localhost
Source Database: ffxiv_server
Target Host: localhost
Target Database: ffxiv_server
Date: 8/20/2016 7:15:35 PM
*/

SET FOREIGN_KEY_CHECKS=0;
-- ----------------------------
-- Table structure for supportdesk_faqs
-- ----------------------------
CREATE TABLE `supportdesk_faqs` (
  `slot` tinyint(4) NOT NULL,
  `languageCode` tinyint(4) NOT NULL,
  `title` varchar(128) NOT NULL,
  `body` text NOT NULL,
  PRIMARY KEY (`slot`,`languageCode`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- ----------------------------
-- Records 
-- ----------------------------
INSERT INTO `supportdesk_faqs` VALUES ('0', '1', 'Welcome to FFXIV Classic', 'Welcome to the FFXIV 1.0 server emulator FFXIVClassic!\r\n\r\nThis is still currently a work in progress, and you may find bugs or issues as you play with this server. Keep in mind that this is not even remotely close to being finished, and that it is a work in progress.\r\n\r\nCheck out the blog at: \r\nhttp://ffxivclassic.fragmenterworks.com/ \r\nCheck out videos at: \r\nhttps://www.youtube.com/channel/UCr2703_er1Dj7Lx5pzpQpfg');
