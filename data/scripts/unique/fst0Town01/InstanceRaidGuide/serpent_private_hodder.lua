require ("global")

function init(npc)
	return false, false, 0, 0, 0x1eb, 0x2;	
end

function onEventStarted(player, npc)    
	callClientFunction(player, "askEnterInstanceRaid", 15);
	player:endEvent();
end