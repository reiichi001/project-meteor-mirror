/*
MySQL Data Transfer
Source Host: localhost
Source Database: ffxiv_server
Target Host: localhost
Target Database: ffxiv_server
Date: 8/20/2016 7:15:41 PM
*/

SET FOREIGN_KEY_CHECKS=0;
-- ----------------------------
-- Table structure for supportdesk_issues
-- ----------------------------
CREATE TABLE `supportdesk_issues` (
  `slot` smallint(4) unsigned NOT NULL,
  `title` varchar(50) NOT NULL,
  PRIMARY KEY (`slot`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- ----------------------------
-- Records 
-- ----------------------------
INSERT INTO `supportdesk_issues` VALUES ('0', 'Report Harassment');
INSERT INTO `supportdesk_issues` VALUES ('1', 'Report Cheating');
INSERT INTO `supportdesk_issues` VALUES ('2', 'Report a Bug or Glitch');
INSERT INTO `supportdesk_issues` VALUES ('3', 'Leave Suggestion');
