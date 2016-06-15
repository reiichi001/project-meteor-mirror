
function onEventStarted(player, npc)
	defaultSea = GetStaticActor("DftSea");
	player:RunEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithEstrilda_001", nil, nil, nil); --DEFAULT
	--player:RunEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithEstrilda_002", nil, nil, nil); --IF ARCHER
	--player:RunEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithEstrilda_003", nil, nil, nil); --IF ARCHER
end