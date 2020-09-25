require ("global")

function onEventStarted(player, npc)
    defaultFst = GetStaticActor("DftFst");
	callClientFunction(player, "delegateEvent", player, defaultFst, "defaultTalkWithGuildleveClientG_001", nil, nil, nil);
	player:endEvent();
end