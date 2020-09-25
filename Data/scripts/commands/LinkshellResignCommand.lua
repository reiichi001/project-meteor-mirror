--[[

LinkshellLeaveCommand Script

--]]

function onEventStarted(player, actor, triggerName, linkshellName)

	GetWorldManager():RequestWorldLinkshellLeave(player, linkshellName);	
	player:EndEvent();
	
end