
function onEventStarted(player, npc)
	defaultSea = getStaticActor("DftSea");
	player:runEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithFrithuric_001", nil, nil, nil);
	--player:runEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithFrithuric_002", nil, nil, nil); --LTW
	--player:runEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithFrithuric_003", nil, nil, nil); --LTW NO GUILD
end