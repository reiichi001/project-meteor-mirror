--[[

RetainerFurniture Script

Functions:

eventPushStepOpenRetainerMenu() - Opens menu to choose retainer
eventRingBell() - Plays the bell ring animation
eventPushRetainerCallCaution() - Shows warning that a open bazaar will be closed if retainer chosen
eventTalkRetainerMenu(hasPossessions, showDispatchChoice) - Opens retainer menu.
eventTalkRetainerDismissal(hasPossessions) - Show dismiss confirmation.
eventTalkRetainerMannequin(0:Enable/1:Disable) - Shows dialog to enable/disable modeling.
eventTalkRetainerItemTrade(?) - ??Opens retainer storage??
eventTalkRetainerItemList(?) -  ??Opens bazaar??
eventReturnResult(?, ?) - ??Trade related??
eventTalkSelectBazaarStreet(limitsWardChoices) - Shows the dialog to send a retainer to a street. Set to 20. 
eventTalkFinish() - Finishs the talk with retainer
eventPlayerTurn(rotation) - Turns the player
--]]

require ("global")

function init(npc)
	return false, false, 0, 0;	
end

function onEventStarted(player, npc, triggerName)
	retainerNumber = callClientFunction(player, "eventPushRetainerCallCaution", true, false);
	--player:SpawnMyRetainer(npc, retainerNumber);
	--callClientFunction(player, "eventRingBell");	
	player:EndEvent();	
end