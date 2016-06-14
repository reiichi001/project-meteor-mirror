
function onEventStarted(player, npc)
	defaultSea = getStaticActor("DftSea");
	player:runEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithNanaka_001", nil, nil, nil);
	--player:runEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithNanaka_002", nil, nil, nil); --GSM
	--player:runEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithNanaka_003", nil, nil, nil); --GSM NO GUILD
end