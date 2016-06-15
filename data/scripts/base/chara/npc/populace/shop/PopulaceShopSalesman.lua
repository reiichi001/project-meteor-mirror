function init(npc)
	return false, false, 0, 0;	
end

function onEventStarted(player, npc)
	player:SendMessage(0x20, "", "This PopulaceShopSalesman actor has no event set. Actor Class Id: " .. tostring(npc:GetActorClassId()))
	player:EndEvent();
	--player:RunEventFunction("welcomeTalk");
end

function onEventUpdate(player, npc, step, menuOptionSelected, lsName, lsCrest)

	player:EndEvent();	
	
end