require ("global")

function onEventStarted(player, npc)
    defaultWil = GetStaticActor("DftWil");
	callClientFunction(player, "delegateEvent", player, defaultWil, "defaultTalkWithFyrilsunn_001", nil, nil, nil);
	player:endEvent();
end