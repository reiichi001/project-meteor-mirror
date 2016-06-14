function onEventStarted(player, npc)
	defaultWil = getStaticActor("DftWil");
	player:runEventFunction("delegateEvent", player, defaultWil, "defaultTalkWithZoengterbin_001", nil, nil, nil);
	--player:runEventFunction("delegateEvent", player, defaultWil, "defaultTalkWithZoengterbin_002", nil, nil, nil);
	--player:runEventFunction("delegateEvent", player, defaultWil, "defaultTalkWithZoengterbin_003", nil, nil, nil);
end

function onEventUpdate(player, npc, blah, menuSelect)
	
	player:endEvent();
	
end