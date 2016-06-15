
function init(npc)
	return false, false, 0, 0;	
end

function onEventStarted(player, npc)
	player:SendMessage(0x20, "", "This PopulaceStandard actor has no event set. Actor Class Id: " .. tostring(npc:GetActorClassId()));
	player:EndEvent();
end

function onEventUpdate(player, npc, blah, menuSelect)
	player:EndEvent();
end