function init(npc)
	return false, false, 0, 0;	
end

function onEventStarted(player, npc, triggerName)
	player:EndEvent();
end

function onEventUpdate(player, npc)
end