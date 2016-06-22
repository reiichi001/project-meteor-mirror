require ("global")
require ("quests/man/man0u0")

function onEventStarted(player, npc, triggerName)
	player:EndEvent();
end