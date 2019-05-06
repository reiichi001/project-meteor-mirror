--[[

TradeOfferCommand Script

Handles what happens when you invite to trade

--]]

function onEventStarted(player, actor, triggerName, name, arg1, arg2, arg3, actorId)	

	local otherActor = nil;

	if (name ~= nil) then
		otherActor = player:GetZone():FindPCInZone(name);
	elseif (actorId ~= nil) then
		otherActor = player:GetZone():FindActorInArea(actorId);
	end
	
	if (otherActor ~= nil) then
		GetWorldManager():CreateTradeGroup(player, otherActor);
	else
	end
	
	player:EndEvent();
	
end