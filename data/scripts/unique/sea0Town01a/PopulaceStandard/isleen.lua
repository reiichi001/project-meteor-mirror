function onEventStarted(player, npc)
	defaultSea = getStaticActor("DftSea");
	player:runEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithIsleen_001", nil, nil, nil);
end

function onEventUpdate(player, npc)
	player:endEvent();
end