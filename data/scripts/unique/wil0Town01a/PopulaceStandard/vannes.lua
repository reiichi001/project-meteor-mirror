require ("global")

function onEventStarted(player, npc)
    defaultWil = GetStaticActor("DftWil");
	callClientFunction(player, "delegateEvent", player, defaultSea, "defaultTalkWithVannes_001", nil, nil, nil);
	player:endEvent();
end
