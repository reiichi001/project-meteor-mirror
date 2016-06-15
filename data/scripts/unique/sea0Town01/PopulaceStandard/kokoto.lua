
function onEventStarted(player, npc)
	defaultSea = GetStaticActor("DftSea");
	player:RunEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithKokoto_001", nil, nil, nil);
	--player:RunEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithKokoto_002", nil, nil, nil); --LNC
	--player:RunEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithKokoto_003", nil, nil, nil); --LNC NO GUILD
end