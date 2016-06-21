require ("global")

function onEventStarted(player, npc)
    defaultWil = GetStaticActor("DftWil");
	callClientFunction(player, "delegateEvent", player, defaultSea, "defaultTalkWithNogeloix_001", nil, nil, nil);
	player:endEvent();
end