--[[

PartyInviteCommand Script

Handles what happens when you invite

--]]

function onEventStarted(player, actor, triggerName, name, arg1, arg2, arg3, actorId)

	if (name ~= nil) then
		getWorldManager():CreateInvitePartyGroup(player, name);
	elseif (actorId ~= nil) then
		getWorldManager():CreateInvitePartyGroup(player, actorId);
	end
	
	player:endCommand();
	
end