
function init(npc)
	return false, false, 0, 0;	
end

function onEventStarted(player, npc)
	player:sendMessage(0x20, "", "This PopulaceStandard actor has no event set.")
	player:endEvent();
end