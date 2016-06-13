function init(npc)
	return "/Chara/Npc/Populace/Shop/PopulaceShopSalesman", false, false, false, false, false, npc.getActorClassId(), false, false, 0, 1, "TEST";	
end

function onEventStarted(player, npc)
	player:runEventFunction("welcomeTalk");
end

function onEventUpdate(player, npc, step, menuOptionSelected, lsName, lsCrest)

	player:endEvent();	
	
end