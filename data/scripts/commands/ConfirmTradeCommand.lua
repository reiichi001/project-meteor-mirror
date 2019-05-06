--[[

ConfirmTradeCommand Script

Handles what happens when you accept/refuse a trade

--]]

function onEventStarted(player, actor, triggerName, groupType, result)

	--Accept
	if (result == 1) then
		GetWorldManager():AcceptTrade(player);
	--Refuse
	elseif (result == 2) then
		GetWorldManager():RefuseTrade(player);
	end
	player:EndEvent();
	
end