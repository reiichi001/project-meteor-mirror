--[[

TradeOfferCommand Script

Handles what happens a player cancels a trade

--]]

function onEventStarted(player, actor, triggerName, commandId, result)	

	GetWorldManager():CancelTrade(player);
	player:EndEvent();
	
end