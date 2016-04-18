function init(npc)
	return "/Chara/Npc/Populace/PopulaceStandard", false, false, false, false, false, npc.getActorClassId(), false, false, 0, 1, "TEST";	
end

function onEventStarted(player, npc)
	defaultSea = getStaticActor("DftSea");
	player:runEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithEstrilda_001", nil, nil, nil); --DEFAULT
	--player:runEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithEstrilda_002", nil, nil, nil); --IF ARCHER
	--player:runEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithEstrilda_003", nil, nil, nil); --IF ARCHER
end

function onEventUpdate(player, npc)
	player:endEvent();
end