
function onEventStarted(player, npc)
	defaultSea = getStaticActor("DftSea");
	player:runEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithKakamehi_001", nil, nil, nil);
	--player:runEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithKakamehi_002", nil, nil, nil); --IF ALC
	--player:runEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithKakamehi_003", nil, nil, nil); --IF ALC
end