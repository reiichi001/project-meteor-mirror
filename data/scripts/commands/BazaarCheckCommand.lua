--[[

BazaarCheckCommand Script

Handles what happens when you examine a player's bazaar

--]]

require ("global")

function onEventStarted(player, actor, triggerName, name, arg1, arg2, arg3, bazaarActorId)	

	local bazaarActor = nil;

	if (name ~= nil) then
		bazaarActor = player:GetZone():FindPCInZone(name);
	elseif (bazaarActorId ~= nil) then
		bazaarActor = player:GetZone():FindActorInArea(bazaarActorId);
	end
	
	if (bazaarActor ~= nil) then
		player:SendMessage(MESSAGE_TYPE_SYSTEM_ERROR, "", "Currently disabled due to freezing characters.");
		--callClientFunction(player, "delegateCommand", GetStaticActor("BazaarCheckCommand"), "processChackBazaar");		
	else
		--Show error
	end
	
	player:EndEvent();
	
end