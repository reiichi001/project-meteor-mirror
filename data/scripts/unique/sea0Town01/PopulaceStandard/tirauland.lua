
function onEventStarted(player, npc)
	defaultSea = GetStaticActor("DftSea");
	player:RunEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithTirauland_001", nil, nil, nil);
	--player:RunEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithTirauland_002", nil, nil, nil); --LNC
	--player:RunEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithTirauland_003", nil, nil, nil); --LNC NO GUILD
	--player:RunEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithTirauland_010", nil, nil, nil); --NOT DOW/DOM
end