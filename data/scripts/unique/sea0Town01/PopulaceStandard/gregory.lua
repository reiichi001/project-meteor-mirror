
function onEventStarted(player, npc)
	defaultSea = GetStaticActor("DftSea");
	player:RunEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithGregory_001", nil, nil, nil);
	--player:RunEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithGregory_002", nil, nil, nil); --CNJ
	--player:RunEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithGregory_003", nil, nil, nil); --CNJ NO GUILD
end