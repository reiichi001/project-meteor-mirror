function init(npc)
	return "/Chara/Npc/Populace/PopulaceStandard", false, false, false, false, false, npc.getActorClassId(), false, false, 0, 1, "TEST";	
end

function onEventStarted(player, npc)
	defaultSea = getStaticActor("DftSea");
	player:runEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithTirauland_001", nil, nil, nil);
	--player:runEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithTirauland_002", nil, nil, nil); --LNC
	--player:runEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithTirauland_003", nil, nil, nil); --LNC NO GUILD
	--player:runEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithTirauland_010", nil, nil, nil); --NOT DOW/DOM
end

function onEventUpdate(player, npc)
	player:endEvent();
end