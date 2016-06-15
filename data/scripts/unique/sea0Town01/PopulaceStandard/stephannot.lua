
function onEventStarted(player, npc)
	defaultSea = GetStaticActor("DftSea");
	player:RunEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithStephannot_001", nil, nil, nil);
	--player:RunEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithStephannot_002", nil, nil, nil); --MIN
	--player:RunEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithStephannot_003", nil, nil, nil); --MIN NO GUILD
end