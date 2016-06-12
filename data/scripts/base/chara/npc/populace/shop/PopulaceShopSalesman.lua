function init(npc)
	return false, false, 0, 0;	
end

function onEventStarted(player, npc)
	player:runEventFunction("welcomeTalk");
end

function onEventUpdate(player, npc, step, menuOptionSelected, lsName, lsCrest)

	player:endEvent();	
	
end