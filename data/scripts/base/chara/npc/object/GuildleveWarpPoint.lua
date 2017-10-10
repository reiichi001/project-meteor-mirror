--[[

GuildleveWarpPoint Script

Functions:

eventGuildleveReward(glId, completionTimeSec, completeReward, difficultyBonus, faction, gil???, factionBonus, RewardId1, RewardAmount1, RewardId2, RewardAmount2, difficulty) - Open Reward Dialog
eventTalkGuildleveWarp(returnAetheryteID1, returnAetheryte2) - Opens choice menu
--]]

require ("global")
require ("aetheryte")
require ("utils")

function init(npc)
	return false, false, 0, 0;	
end

function onEventStarted(player, npc, triggerName)
	local currentGLDirector = player:GetGuildleveDirector();
	local glData = currentGLDirector.guildleveData;

	callClientFunction(player, "eventGuildleveReward", currentGLDirector.guildleveId, currentGLDirector.completionTime, 24, 24, 0, 0, 0, 0, 0, 0, 0, currentGLDirector.selectedDifficulty);
	
	local choice = callClientFunction(player, "eventTalkGuildleveWarp", glData.aetheryte, 0);
	
	if (choice == 3) then
		local destination = aetheryteTeleportPositions[glData.aetheryte];
		if (destination ~= nil) then
			randoPos = getRandomPointInBand(destination[2], destination[4], 3, 5);
			rotation = getAngleFacing(randoPos.x, randoPos.y, destination[2], destination[4]);
			GetWorldManager():DoZoneChange(player, destination[1], nil, 0, 2, randoPos.x, destination[3], randoPos.y, rotation);
			currentGLDirector:EndDirector();
		end
	elseif (choice == 4) then
		currentGLDirector:EndDirector();
	end
	
	player:EndEvent();
end

--50023: GL COMPLETE!
--50132: You earn faction credits from X