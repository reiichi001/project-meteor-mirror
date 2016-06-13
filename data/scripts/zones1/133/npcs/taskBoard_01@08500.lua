function init(npc)
	return "/Chara/Npc/Object/TaskBoard", false, false, false, false, false, npc.getActorClassId(), false, false, 0, 1, "TEST";	
end

function onEventStarted(player, npc)
	player:endEvent();	
end

function onEventUpdate(player, npc, step, menuOptionSelected, lsName, lsCrest)
	player:endEvent();	
end