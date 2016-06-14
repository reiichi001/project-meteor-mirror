
function onEventStarted(player, npc)
	defaultSea = getStaticActor("DftSea");
	player:runEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithStephannot_001", nil, nil, nil);
	--player:runEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithStephannot_002", nil, nil, nil); --MIN
	--player:runEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithStephannot_003", nil, nil, nil); --MIN NO GUILD
end