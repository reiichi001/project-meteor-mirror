require ("global")
require ("guildleve")

--DirectorId, GuildleveId, Aetheryte Location (6 or ~6), exMarkerX, exMarkerY, exMarkerZ

--22: Limsa Battle Leve
--14: Gridania Battle Leve
--26: Uldah Battle Leve
--16: Coerthas Faction Leve
--72: Harvest Leve

function init(thisDirector)	
	return "/Director/Guildleve/PrivateGLBattleSweepNormal", 0x4e25, thisDirector.guildleveId, 6, 0, 0, 0;
end

function main(thisDirector)

	guildleveData = GetGuildleveGamedata(thisDirector.guildleveId);
	members = thisDirector:GetPlayerMembers();
	
	if (members ~= nil and #members ~= 0) then
		player = members[0];
		player:SendGameMessage(GetWorldMaster(), 50036, 0x20, thisDirector.guildleveId, player, 0); --"You have started the leve..."
		player:PlayAnimation(getGLStartAnimationFromSheet(guildleveData.borderId, guildleveData.plateId, false));
	end

	wait(3);
	
	thisDirector:StartGuildleve();
	thisDirector:SyncAllInfo();	
	thisDirector:UpdateMarkers(0, 59.0, 44.0, -163.0);
	
	if (members ~= nil and #members ~= 0) then
		player = members[0];
		
		player:ChangeMusic(22);
		attentionMessage(player, 50022, thisDirector.guildleveId, thisDirector.selectedDifficulty, 0);
		player:SendGameMessage(GetWorldMaster(), 50026, 0x20, guildleveData.timeLimit);
	end
	
	wait(5);
	
	thisDirector:UpdateAimNumNow(0, 1);
	wait(3);
	thisDirector:UpdateAimNumNow(0, 2);
	wait(3);
	thisDirector:UpdateAimNumNow(0, 3);
	
end

function attentionMessage(player, textId, ...)
	player:SendGameMessage(GetWorldMaster(), textId, 0x20, ...);
	player:SendDataPacket("attention", GetWorldMaster(), "", textId, ...);
end
