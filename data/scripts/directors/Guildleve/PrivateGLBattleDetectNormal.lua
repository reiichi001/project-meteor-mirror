require ("global")
require ("guildleve")

--DirectorId, GuildleveId, Aetheryte Location (6 or ~6), exMarkerX, exMarkerY, exMarkerZ

local guildleveId;

function init(thisDirector, glId)
	guildleveId = glId;
	
	guildleveData = GetGuildleveGamedata(glId);
	members = thisDirector:GetPlayerMembers();
	
	if (members ~= nil and #members ~= 0) then
		player = members[0];
		player:SendGameMessage(GetWorldMaster(), 50036, 0x20, glId, player, 0); --"You have started the leve..."
		player:PlayAnimation(getGLStartAnimationFromSheet(guildleveData.borderId, guildleveData.plateId, false));
	end
	
	return "/Director/Guildleve/PrivateGLBattleDetectNormal", 0x4e25, glId, 6, 0, 0, 0;
end

function mainLoop(thisDirector)

	
	
end