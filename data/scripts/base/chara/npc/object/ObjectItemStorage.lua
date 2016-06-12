function init(npc)
	return false, false, 0, 0;	
end

function onEventStarted(player, npc, triggerName)
	player:endEvent();
end

function onEventUpdate(player, npc)
end