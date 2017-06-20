/*
MySQL Data Transfer
Source Host: localhost
Source Database: ffxiv_server
Target Host: localhost
Target Database: ffxiv_server
Date: 6/19/2017 10:23:42 PM
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
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1090460', '10013', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1090461', '10013', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1090462', '10013', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1290007', '10006', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1290008', '10006', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1290009', '10006', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1200052', '20001', '0', '6');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1200055', '20006', '0', '6');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1200053', '20005', '0', '6');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1200057', '20007', '0', '6');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1200054', '20002', '0', '6');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1200056', '20003', '0', '6');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280000', '10002', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280001', '10002', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280002', '10002', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280003', '10002', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280004', '10002', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280005', '10002', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280006', '10002', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280007', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280008', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280009', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280010', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280011', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280012', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280013', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280014', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280015', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280016', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280017', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280018', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280019', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280020', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280021', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280022', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280023', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280024', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280025', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280026', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280027', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280028', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280029', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280030', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280031', '10002', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280032', '10002', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280033', '10002', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280034', '10002', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280035', '10002', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280036', '10002', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280037', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280038', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280039', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280040', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280041', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280042', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280043', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280044', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280045', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280046', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280047', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280048', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280049', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280050', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280051', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280052', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280053', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280054', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280055', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280056', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280057', '10002', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280058', '10002', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280059', '10002', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280060', '10002', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280061', '10002', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280062', '10002', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280063', '10002', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280064', '10002', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280065', '10002', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280066', '10002', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280067', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280068', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280069', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280070', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280071', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280072', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280073', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280074', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280075', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280076', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280077', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280078', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280079', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280080', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280081', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280082', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280083', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280084', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280085', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280086', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280087', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280088', '10002', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280089', '10002', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280090', '10002', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280091', '10002', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280092', '10002', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280093', '10002', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280094', '10002', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280095', '10002', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280096', '10002', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280097', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280098', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280099', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280100', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280101', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280102', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280103', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280104', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280105', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280106', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280107', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280108', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280109', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280110', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280111', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280112', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280113', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280114', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280115', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280116', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280117', '10002', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280118', '10002', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280119', '10002', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280120', '10002', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280121', '10002', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280122', '10002', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280123', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280124', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280125', '10010', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280126', '10002', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1280127', '10002', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1200027', '10008', '0', '8');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1200040', '10003', '0', '12');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1090547', '10011', '0', '10');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1090548', '10011', '0', '10');
INSERT INTO `gamedata_actor_pushcommand` VALUES ('1090549', '10011', '0', '10');
