--[[

LinkshellKickCommand Script

--]]

function onEventStarted(player, actor, triggerName, linkshellName, kickedName)

	GetWorldManager():RequestWorldLinkshellKick(player, linkshellName, kickedName);	
	player:EndEvent();
	
end