require ("global")
require ("quests/man/man0g0")

function onEventStarted(player, npc, triggerName)
	if (triggerName == "caution") then
		worldMaster = GetWorldMaster();
		player:SendGameMessage(player, worldMaster, 34109, 0x20);
	elseif (triggerName == "exit") then
		GetWorldManager():DoPlayerMoveInZone(player, 356.09, 3.74, -701.62, -1.4);
	end
	player:EndEvent();
end