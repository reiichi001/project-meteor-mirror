
function onEventStarted(player, npc)
	defaultSea = GetStaticActor("DftSea");
	player:RunEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithKakamehi_001", nil, nil, nil);
	--player:RunEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithKakamehi_002", nil, nil, nil); --IF ALC
	--player:RunEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithKakamehi_003", nil, nil, nil); --IF ALC
end