--[[

PopulaceChocoboLender Script

Functions:

eventTalkWelcome(player) - Start Text
eventAskMainMenu(player, curLevel, hasFundsForRent, isPresentChocoboIssuance, isSummonMyChocobo, isChangeBarding, currentChocoboWare) - Shows the main menu
eventTalkMyChocobo(player) - Starts the cutscene for getting a chocobo
eventSetChocoboName(true) - Opens the set name dialog
eventAfterChocoboName(player) - Called if player done naming chocobo, shows cutscene, returns state and waits to teleport outside city.
eventCancelChocoboName(player) - Called if player cancels naming chocobo, returns state. 
eventTalkStepBreak(player) - Finishes talkTurn and says a goodbye
--]]

require ("global")

function init(npc)
	return false, false, 0, 0;	
end

function onEventStarted(player, npc, triggerName)
	
	--callClientFunction(player, "eventTalkWelcome", player);
	callClientFunction(player, "eventAskMainMenu", player, 20, true, true, true, true, 4);
	callClientFunction(player, "eventTalkMyChocobo", player);
	callClientFunction(player, "eventSetChocoboName", false);
	callClientFunction(player, "eventAfterChocoboName", player);
	
	player:EndEvent();
end