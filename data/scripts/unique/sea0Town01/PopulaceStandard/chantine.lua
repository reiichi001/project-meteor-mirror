
function onEventStarted(player, npc)
	defaultSea = getStaticActor("DftSea");
	player:runEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithChantine_001", nil, nil, nil);
	--player:runEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithChantine_002", nil, nil, nil); --LNC
	--player:runEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithChantine_003", nil, nil, nil); --LNC NO GUILD
end