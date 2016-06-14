function init(npc)
	return false, false, 0, 0;	
end

function onEventStarted(player, npc, triggerName)
	if (triggerName == "caution") then
		worldMaster = getWorldMaster();
		player:sendGameMessage(player, worldMaster, 34109, 0x20);
	elseif (triggerName == "exit") then
		getWorldManager():DoPlayerMoveInZone(player, 5);
	end
	player:endEvent();
end