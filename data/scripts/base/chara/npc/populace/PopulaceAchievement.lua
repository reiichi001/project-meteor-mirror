--[[

PopulaceAchievement Script

Functions:

eventNoGC() - 
eventUnlock(sheetId) - 
eventReward(?, bool, ?, bool) - 
defTalk() - Blurb

--]]

require ("global")

function init(npc)
	return false, false, 0, 0;	
end

function onEventStarted(player, npc, triggerName)
	
	callClientFunction(player, "defTalk");
	
	player:EndEvent();
end