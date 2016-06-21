require ("global")
require ("quests/man/man0l0")

function onEventStarted(player, npc, triggerName)
	man0l0Quest = GetStaticActor("Man0l0");
	
	callClientFunction(player, "delegateEvent", player, man0l0Quest, "processEvent000_15", nil, nil, nil, nil);
	
	player:EndEvent();
end