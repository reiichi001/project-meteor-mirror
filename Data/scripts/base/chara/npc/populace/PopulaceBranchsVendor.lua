--[[

PopulaceBranchVendor Script

Functions:

eventTalkWelcome(player) - Starts talk turn and 
eventSearchItemAsk(nil, stopSearchingItemId) - 
eventTalkStepBreak() - Finishes the talk turn.

--]]

require ("global")

function init(npc)
	return false, false, 0, 0;	
end

function onEventStarted(player, npc, triggerName)
	
	callClientFunction(player, "eventTalkWelcome", player);	
	callClientFunction(player, "eventSearchItemAsk", nil, 0);	
	callClientFunction(player, "eventTalkStepBreak", player);	
	player:EndEvent();
	
end