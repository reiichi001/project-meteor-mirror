
function onEventStarted(player, npc)
	defaultSea = GetStaticActor("DftSea");
	player:RunEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithNanaka_001", nil, nil, nil);
	--player:RunEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithNanaka_002", nil, nil, nil); --GSM
	--player:RunEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithNanaka_003", nil, nil, nil); --GSM NO GUILD
end