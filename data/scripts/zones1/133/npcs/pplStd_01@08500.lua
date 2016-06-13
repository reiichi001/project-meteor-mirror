function init(npc)
	return "/Chara/Npc/Populace/PopulaceStandard", false, false, false, false, false, npc.getActorClassId(), false, false, 0, 1, "TEST";	
end

function onEventStarted(player, npc)
	defaultSea = getStaticActor("DftSea");
	player:runEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithKakamehi_001", nil, nil, nil);
	--player:runEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithKakamehi_002", nil, nil, nil); --IF ALC
	--player:runEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithKakamehi_003", nil, nil, nil); --IF ALC
end

function onEventUpdate(player, npc)
	player:endEvent();
end