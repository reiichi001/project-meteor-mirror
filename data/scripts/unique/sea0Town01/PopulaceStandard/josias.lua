
function onEventStarted(player, npc)
	defaultSea = GetStaticActor("DftSea");
	player:RunEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithJosias_001", nil, nil, nil);
	--player:RunEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithJosias_002", nil, nil, nil);  --CRP
	--player:RunEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithJosias_003", nil, nil, nil);  --CRP NO GUILD
end