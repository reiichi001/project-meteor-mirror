function onInstantiate(npc)
	return "/Chara/Npc/Populace/PopulaceTutorial", false, false, false, false, false, npc.getActorClassId(), false, false, 0, 1, "TEST";	
end

function onEventStarted(player, npc, triggerName)
	man0l0Quest = getStaticActor("Man0l0");
	player:runEventFunction("delegateEvent", player, man0l0Quest, "processTtrNomal003", nil, nil, nil);
	--player:runEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithFrithuric_002", nil, nil, nil); --LTW
	--player:runEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithFrithuric_003", nil, nil, nil); --LTW NO GUILD
end

function onEventUpdate(player, npc)	
end