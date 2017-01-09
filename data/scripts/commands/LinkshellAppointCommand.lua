--[[

LinkshellAppointCommand Script

--]]

function onEventStarted(player, actor, triggerName, linkshellName, memberName, rank)

	GetWorldManager():RequestWorldLinkshellRankChange(player, linkshellName, memberName, rank);	
	player:EndEvent();
	
end