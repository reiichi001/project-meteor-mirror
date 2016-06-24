--[[

CheckCommand Script

Handles player examining someone

--]]

function onEventStarted(player, commandactor, triggerName, arg1, arg2, arg3, arg4, checkedActorId)

	actor = player:GetActorInInstance(checkedActorId);
	
	if (actor ~= nil) then
		player:examinePlayer(actor);
	end
	
	player:EndEvent();
	
end
