
function onEventStarted(player, npc)
	defaultSea = getStaticActor("DftSea");
	player:runEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithKokoto_001", nil, nil, nil);
	--player:runEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithKokoto_002", nil, nil, nil); --LNC
	--player:runEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithKokoto_003", nil, nil, nil); --LNC NO GUILD
end