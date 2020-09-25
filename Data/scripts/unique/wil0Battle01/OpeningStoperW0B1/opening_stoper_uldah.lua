function init(npc)
	return false, false, 0, 0;	
end

function onEventStarted(player, npc, triggerName)
	if (triggerName == "caution") then
		worldMaster = GetWorldMaster();
		player:SendGameMessage(player, worldMaster, 34109, 0x20);
	elseif (triggerName == "exit") then
		GetWorldManager():DoPlayerMoveInZone(player, 5.36433, 196, 133.656, -2.84938);
	end
	player:EndEvent();
end