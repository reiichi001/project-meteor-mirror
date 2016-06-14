function onEventStarted(player, npc)
	defaultWil = getStaticActor("DftWil");
	player:runEventFunction("delegateEvent", player, defaultWil, "defaultTalkWithOpondhao_001", nil, nil, nil);
	--player:runEventFunction("delegateEvent", player, defaultWil, "defaultTalkWithOpondhao_002", nil, nil, nil);
	--player:runEventFunction("delegateEvent", player, defaultWil, "defaultTalkWithOpondhao_003", nil, nil, nil);
end

function onEventUpdate(player, npc, blah, menuSelect)
	
	player:endEvent();
	
end