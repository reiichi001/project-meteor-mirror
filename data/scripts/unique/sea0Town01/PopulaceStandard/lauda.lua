
function onEventStarted(player, npc)
	defaultSea = GetStaticActor("DftSea");
	player:RunEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithLauda_001", nil, nil, nil);
	--player:RunEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithLauda_002", nil, nil, nil); --BTN
	--player:RunEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithLauda_003", nil, nil, nil); --BTN NO GUILD
end