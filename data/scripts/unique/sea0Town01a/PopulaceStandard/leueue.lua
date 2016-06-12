function onEventStarted(player, npc)
	defaultSea = getStaticActor("DftSea");
	player:runEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithInn_Desk", nil, nil, nil);
end

function onEventUpdate(player, npc)
	player:endEvent();
end