function onEventStarted(player, npc)
	defaultWil = getStaticActor("DftWil");
	player:runEventFunction("delegateEvent", player, defaultWil, "defaultTalkWithYhahamariyo_001", nil, nil, nil);
	--player:runEventFunction("delegateEvent", player, defaultWil, "defaultTalkWithYhahamariyo_002", nil, nil, nil);
	--player:runEventFunction("delegateEvent", player, defaultWil, "defaultTalkWithYhahamariyo_003", nil, nil, nil);
	
end

function onEventUpdate(player, npc, blah, menuSelect)
	
	player:endEvent();
	
end