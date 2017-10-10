require ("global")

function onEventStarted(player, npc, triggerName)
    defaultFst = GetStaticActor("DftFst");
	callClientFunction(player, "delegateEvent", player, defaultFst, "defaultTalkWithVkorolon_001");
	player:EndEvent();

end