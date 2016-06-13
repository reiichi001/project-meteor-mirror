function init(npc)
	return "/Chara/Npc/Populace/PopulaceStandard", false, false, false, false, false, npc.getActorClassId(), false, false, 0, 1, "TEST";	
end

function onEventStarted(player, npc)
	defaultSea = getStaticActor("DftSea");
	player:runEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithFrithuric_001", nil, nil, nil);
	--player:runEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithFrithuric_002", nil, nil, nil); --LTW
	--player:runEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithFrithuric_003", nil, nil, nil); --LTW NO GUILD
end

function onEventUpdate(player, npc)
	player:endEvent();
end