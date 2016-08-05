require ("global")

function onEventStarted(player, npc)
    defaultFst = GetStaticActor("DftFst");
	callClientFunction(player, "delegateEvent", player, defaultFst, "defaultTalkWithSerpent_sergeant_frilaix_001", nil, nil, nil);
	player:endEvent();
end