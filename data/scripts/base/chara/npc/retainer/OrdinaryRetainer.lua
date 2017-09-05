--[[

OrdinaryRetainer Script

Functions:

eventTalkRetainerOther() - 
eventTalkRetainerMenu(mode, hasPossessions) - Opens the main menu. If mode == 2, hide dismiss option.
eventTalkRetainerDismissal(hasPossessions) - Show dismiss confirmation.
eventTalkRetainerMannequin(0:enable/1:disable confirm) - Show bazaar modeling confirmation.
eventTalkRetainerItemTrade(?) - ??Opens retainer storage??
eventTalkRetainerItemList(?) -  ??Opens bazaar??
eventReturnResult(?, ?) - ??Trade related??
sayToPlayer(actorClassId, messageType, argument) - Makes the retainer say a phrase to the player.
eventTalkFinish() - Stops npc from looking at player.
eventPlayerTurn(angle) - Turns player to angle.

--]]

require ("global")

function init(npc)
	return false, false, 0, 0;	
end

function onEventStarted(player, npc, triggerName)
	
	callClientFunction(player, "eventTalkRetainerItemList", 1);
	--callClientFunction(player, "eventTalkRetainerDismissal", 0, false);
	player:EndEvent();
end