--[[

LinkshellInviteCancelCommand Script

Handles what happens when you cancel an invite to a linkshell

--]]

function onEventStarted(player, actor, triggerName, arg1, arg2, arg3, arg4, actorId)

	GetWorldManager():RequestWorldLinkshellCancelInvite(player);	
	player:EndEvent();
	
end