
function onEventStarted(player, npc)
	defaultSea = GetStaticActor("DftSea");
	player:RunEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithFrithuric_001", nil, nil, nil);
	--player:RunEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithFrithuric_002", nil, nil, nil); --LTW
	--player:RunEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithFrithuric_003", nil, nil, nil); --LTW NO GUILD
end