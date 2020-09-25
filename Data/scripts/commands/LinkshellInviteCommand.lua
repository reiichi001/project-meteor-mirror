--[[

LinkshellInviteCommand Script

Handles what happens when you invite a player to a linkshell

--]]

function onEventStarted(player, actor, triggerName, linkshellName, arg1, arg2, arg3, actorId)

	GetWorldManager():RequestWorldLinkshellInviteMember(player, linkshellName, actorId);	
	player:EndEvent();
	
end