require ("global")
require ("quests/man/man0u0")

function onEventStarted(player, npc, triggerName)
	man0u0Quest = GetStaticActor("Man0u0");	
	
	player:EndEvent();
end