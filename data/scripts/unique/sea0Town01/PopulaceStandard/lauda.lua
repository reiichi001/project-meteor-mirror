
function onEventStarted(player, npc)
	defaultSea = getStaticActor("DftSea");
	player:runEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithLauda_001", nil, nil, nil);
	--player:runEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithLauda_002", nil, nil, nil); --BTN
	--player:runEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithLauda_003", nil, nil, nil); --BTN NO GUILD
end