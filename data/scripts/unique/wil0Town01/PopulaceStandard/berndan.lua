function onEventStarted(player, npc)
	defaultWil = GetStaticActor("DftWil");
	player:RunEventFunction("delegateEvent", player, defaultWil, "defaultTalkWithBerndan_001", nil, nil, nil);
end

function onEventUpdate(player, npc, blah, menuSelect)
	
	player:EndEvent();
	
end