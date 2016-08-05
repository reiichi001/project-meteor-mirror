--[[

PopulaceBlackMarketeer Script

Functions:

eventTalkWelcome(player) - Start Text
eventSellItemAsk(player, ?, ?)
eventAskMainMenu(player, index) - Shows 
eventTalkBye(player) - Says bye text
eventTalkStepBreak() - Ends talk

eventSealShopMenuOpen() - 
eventSealShopMenuAsk() - 
eventSealShopMenuClose() - 
eventGilShopMenuOpen() - 
eventGilShopMenuAsk() - 
eventGilShopMenuClose() - 

--]]

require ("global")

function init(npc)
	return false, false, 0, 0;	
end

function onEventStarted(player, npc, triggerName)
	
	callClientFunction(player, "eventTalkWelcome", player);
	
	currancyType = callClientFunction(player, "eventAskMainMenu", player);
	callClientFunction(player, "eventSealShopMenuAsk", player);
	
	
	player:EndEvent();
end