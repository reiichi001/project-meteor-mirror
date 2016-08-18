require ("global")

function onEventStarted(player, npc)
	defaultSea = GetStaticActor("DftSea");
	callClientFunction(player, "delegateEvent", player, defaultSea, "defaultTalkWithClifton_001", nil, nil, nil);
	player:endEvent();
end