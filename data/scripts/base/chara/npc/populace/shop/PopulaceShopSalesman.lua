function init(npc)
	return false, false, 0, 0;	
end

function onEventStarted(player, npc)
	player:sendMessage(0x20, "", "This PopulaceShopSalesman actor has no event set.")
	player:endEvent();
	--player:runEventFunction("welcomeTalk");
end

function onEventUpdate(player, npc, step, menuOptionSelected, lsName, lsCrest)

	player:endEvent();	
	
end