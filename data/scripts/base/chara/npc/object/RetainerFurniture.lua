--[[

RetainerFurniture Script

Functions:

eventPushStepOpenRetainerMenu() - Opens menu to choose retainer
eventRingBell() - Plays the bell ring animation
eventPushRetainerCallCaution() - Shows warning that a open bazaar will be closed if retainer chosen
eventTalkRetainerMenu(hasPossessions, showDispatchChoice) - Opens retainer menu.
eventTalkRetainerDismissal(hasPossessions) - Show dismiss confirmation.
eventTalkRetainerMannequin(0:Enable/1:Disable) - Shows dialog to enable/disable modeling.
eventTalkRetainerItemTrade(operationCode) - Operate RetainerTradeWidget. Codes: 1 - Open, 2 - Select Mode, 3 - Close.
eventTalkRetainerItemList(operationCode) -  Operate Bazaar Widget. Codes: 1 - Open, 2 - Select Mode, 3 - Close.
eventReturnResult(resultCode, ?) - Redraws the RetainerTrade UI.
eventTalkSelectBazaarStreet(limitsWardChoices) - Shows the dialog to send a retainer to a street. Set to 20. 
eventTalkFinish() - Finishs the talk with retainer
eventPlayerTurn(rotation) - Turns the player
--]]

require ("global")
require ("retainer")

function init(npc)
	return false, false, 0, 0;	
end

function onEventStarted(player, npc, triggerName)
	retainerNumber = callClientFunction(player, "eventPushStepOpenRetainerMenu");
	
	if (retainerNumber == nil or retainerNumber == 0) then
		player:EndEvent();
		return;
	end
	
	callClientFunction(player, "eventRingBell");	
	retainer = player:SpawnMyRetainer(npc, retainerNumber);
	
	while (true) do
		choice = callClientFunction(player, "eventTalkRetainerMenu", false, true);
		if (choice == 1) then
			doItemTrade(player, retainer);
		elseif (choice == 2) then
			doBazaar(player, retainer);
		elseif (choice == 7) then
			callClientFunction(player, "eventTalkRetainerMannequin", 0);		
		elseif (choice == 8) then
			callClientFunction(player, "eventTalkSelectBazaarStreet", 20);
		else
			break;
		end
	end
	
	player:DespawnMyRetainer();
	player:EndEvent();
end
