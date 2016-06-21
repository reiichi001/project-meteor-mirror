require ("global")

function onEventStarted(player, npc)
	defaultSea = GetStaticActor("DftSea");
	callClientFunction(player, "delegateEvent", player, defaultSea, "
	player:RunEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithBayard_001", nil, nil, nil);
	player:endEvent();
end