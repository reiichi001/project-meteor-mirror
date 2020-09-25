--[[

PopulaceItemRepairer Script

Functions:

INIT: speechType, townType

talkWelcome(player, bool, number, bool) - Opens the main menu
selectItem(nil, pageNumber, ?, condition1, condition2, condition3, condition4, condition5) - "Ain't running a charity here", message said when you have insufficent funds
confirmRepairItem(player, price, itemId, hq grade) - Shows the confirm box for item repair.
confirmUseFacility(player, price) - Shows confirm box for using facility. Default price is 11k?
finishTalkTurn() - Call at end to stop npc from staring at the player (eeeek)

--]]

require ("global")

function init(npc)
	return false, false, 0, 0;	
end

function onEventStarted(player, npc, triggerName)
	
	result = callClientFunction(player, "eventTalkStep14");
	
	player:EndEvent();
end