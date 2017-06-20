--[[

PopulaceBountyPresenter Script

Functions:

eventLowerLevel(player) -
eventAlreadyPresent(player) - 
eventBeforePresent(player) -
eventAfterPresent(player) -
eventJail(player, bool) -

--]]

require ("global")

function init(npc)
	return false, false, 0, 0;	
end

function onEventStarted(player, npc, triggerName)
	
	callClientFunction(player, "eventLowerLevel", player);	
	player:EndEvent();
	
end