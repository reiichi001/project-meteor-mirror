--[[

RetainerFurniture Script

Functions:

eventPushStepOpenRetainerMenu() - Opens menu to choose retainer
eventRingBell() - Plays the bell ring animation
eventPushRetainerCallCaution() - Shows warning that a open bazaar will be closed if retainer chosen
eventTalkRetainerMenu(?, ?) - Opens retainer menu
eventTalkRetainerDismissal(?)
eventTalkRetainerMannequin(?)
eventTalkRetainerItemTrade(?)
eventTalkRetainerItemList(?)
eventTalkSelectBazaarStreet(?)
eventReturnResult(?, ?)
eventTalkFinish()
eventPlayerTurn(rotation) - Turns the player
--]]

require ("global")

function init(npc)
	return false, false, 0, 0;	
end

function onEventStarted(player, npc, triggerName)
	retainerNumber = callClientFunction(player, "eventPushStepOpenRetainerMenu");
	callClientFunction(player, "eventRingBell");
	callClientFunction(player, "eventTalkRetainerMenu");	
	player:EndEvent();	
end