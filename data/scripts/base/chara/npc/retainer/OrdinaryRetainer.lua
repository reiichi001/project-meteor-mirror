--[[

OrdinaryRetainer Script

Functions:

eventTalkRetainerOther() - 
eventTalkRetainerMenu(mode, hasPossessions) - Opens the main menu. If mode == 2, hide dismiss option.
eventTalkRetainerDismissal(hasPossessions) - Show dismiss confirmation.
eventTalkRetainerMannequin(0:enable/1:disable confirm) - Show bazaar modeling confirmation.
eventTalkRetainerItemTrade(operationCode) - Operate RetainerTradeWidget. Codes: 1 - Open, 2 - Select Mode, 3 - Close.
eventTalkRetainerItemList(operationCode) -  Operate Bazaar Widget. Codes: 1 - Open, 2 - Select Mode, 3 - Close.
eventReturnResult(resultCode, ?) - Redraws the RetainerTrade UI.
sayToPlayer(actorClassId, messageType, argument) - Makes the retainer say a phrase to the player.
eventTalkFinish() - Stops npc from looking at player.
eventPlayerTurn(angle) - Turns player to angle.

--]]

require ("global")
require ("retainer")

function init(npc)
	return false, false, 0, 0;	
end

function onEventStarted(player, retainer, triggerName)
	
	while (true) do
		choice = callClientFunction(player, "eventTalkRetainerMenu", 1);
		if (choice == 1) then
			doItemTrade(player, retainer);
		elseif (choice == 2) then
			doBazaar(player, retainer);
		elseif (choice == 7) then
			callClientFunction(player, "eventTalkRetainerMannequin", 0);		
		elseif (choice == 5) then
			player:DespawnMyRetainer();
		else
			break;
		end
	end
	
	player:EndEvent();
	
end
