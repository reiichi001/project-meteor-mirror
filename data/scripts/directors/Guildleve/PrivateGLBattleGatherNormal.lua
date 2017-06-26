require ("global")
require ("guildleve")

--DirectorId, GuildleveId

function init()
	return "/Director/Guildleve/PrivateGLBattleGatherNormal", 0x4e25, thisDirector.guildleveId, 6, 0, 0, 0;
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
	
end