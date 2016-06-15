
function onEventStarted(player, npc)
	defaultSea = GetStaticActor("DftSea");
	player:RunEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithMaunie_001", nil, nil, nil);
	--player:RunEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithMaunie_002", nil, nil, nil); --PUG
	--player:RunEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithMaunie_003", nil, nil, nil); --PUG NO GUILD
end