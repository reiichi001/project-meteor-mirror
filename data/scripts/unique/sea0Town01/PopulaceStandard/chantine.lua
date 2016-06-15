
function onEventStarted(player, npc)
	defaultSea = GetStaticActor("DftSea");
	player:RunEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithChantine_001", nil, nil, nil);
	--player:RunEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithChantine_002", nil, nil, nil); --LNC
	--player:RunEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithChantine_003", nil, nil, nil); --LNC NO GUILD
end