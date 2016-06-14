
function onEventStarted(player, npc)
	defaultSea = getStaticActor("DftSea");
	player:runEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithMaunie_001", nil, nil, nil);
	--player:runEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithMaunie_002", nil, nil, nil); --PUG
	--player:runEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithMaunie_003", nil, nil, nil); --PUG NO GUILD
end