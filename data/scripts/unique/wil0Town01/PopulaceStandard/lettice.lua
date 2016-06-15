function onEventStarted(player, npc)
	defaultWil = GetStaticActor("DftWil");
	player:RunEventFunction("delegateEvent", player, defaultWil, "defaultTalkWithLettice_001", nil, nil, nil);
	--player:RunEventFunction("delegateEvent", player, defaultWil, "defaultTalkWithLettice_002", nil, nil, nil);
	--player:RunEventFunction("delegateEvent", player, defaultWil, "defaultTalkWithLettice_003", nil, nil, nil);
end

function onEventUpdate(player, npc, blah, menuSelect)
	
	player:EndEvent();
	
end