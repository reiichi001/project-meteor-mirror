require ("global")
require ("quests/man/man0g0")

function onEventStarted(player, npc, triggerName)
	if (triggerName == "caution") then
		worldMaster = GetWorldMaster();
		player:SendGameMessage(player, worldMaster, 34109, 0x20);
	elseif (triggerName == "exit") then
		GetWorldManager():DoPlayerMoveInZone(player, 5);
	end
	player:EndEvent();
end