
function onEventStarted(player, npc)
	defaultSea = getStaticActor("DftSea");
	player:runEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithJosias_001", nil, nil, nil);
	--player:runEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithJosias_002", nil, nil, nil);  --CRP
	--player:runEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithJosias_003", nil, nil, nil);  --CRP NO GUILD
end