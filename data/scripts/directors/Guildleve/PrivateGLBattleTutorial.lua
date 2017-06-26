require ("global")
require ("guildleve")

--DirectorId, GuildleveId, Aetheryte Location (6 or ~6), exMarkerX, exMarkerY, exMarkerZ

--50101: This is a tutorial covering regional levequests for Disciples of War and Disciples of Magic.
--50102: The general location of your target can be determined by using the minimap.
--50105: Your target is nearby. Proceed with the levequest objectives.
--50107: This levequest asks that you exterminate a total of [@VALUE($E8(1))] targets. Try finding the next one.
--50110: Defeating targets will sometimes earn you experience points.
--50112: An aetherial node will appear when levequest objectives have been met. Try approaching it.
--50114: Use the node to collect your reward and teleport back to the starting location of the levequest.

--22: Limsa Battle Leve
--14: Gridania Battle Leve
--26: Uldah Battle Leve
--16: Coerthas Faction Leve
--72: Harvest Leve

function init(thisDirector)	
	return "/Director/Guildleve/PrivateGLBattleTutorial", 0x4e25, thisDirector.guildleveId, 6, 0, 0, 0;
end

function main(thisDirector)

	wait(3);
	
	thisDirector:StartGuildleve();
	thisDirector:SyncAllInfo();	
	thisDirector:UpdateMarkers(0, 59.0, 44.0, -163.0);	
	
	wait(5);
	
	thisDirector:UpdateAimNumNow(0, 1);
	wait(3);
	thisDirector:UpdateAimNumNow(0, 2);
	wait(3);
	thisDirector:UpdateAimNumNow(0, 3);
	
	wait(3);	
	thisDirector:EndGuildleve(true);
	
end

function attentionMessage(player, textId, ...)
	player:SendGameMessage(GetWorldMaster(), textId, 0x20, args);
	player:SendDataPacket("attention", GetWorldMaster(), "", textId, args);
end
