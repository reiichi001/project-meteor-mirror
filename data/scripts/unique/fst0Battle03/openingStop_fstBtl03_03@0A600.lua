function init(npc)
	return "/Chara/Npc/Object/OpeningStoperF0B1", false, false, false, false, false, npc.GetActorClassId(), false, false, 0, 1, "TEST";	
end

function onEventStarted(player, npc, triggerName)
	if (triggerName == "caution") then
		worldMaster = GetWorldMaster();
		player:SendGameMessage(player, worldMaster, 34109, 0x20);
	elseif (triggerName == "exit") then
		GetWorldManager():DoPlayerMoveInZone(player, 5);
	end
	player:EndEvent();
end