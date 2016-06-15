function init(npc)
	return false, false, 0, 0;	
end

function onEventStarted(player, npc, triggerName)
	player:RunEventFunction("materiabookTalk");
end

function onEventUpdate(player, npc, step, menuOptionSelected)
	player:EndEvent();	
end