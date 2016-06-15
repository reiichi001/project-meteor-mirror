function onEventStarted(player, npc)
	defaultWil = GetStaticActor("DftWil");
	player:RunEventFunction("delegateEvent", player, defaultWil, "defaultTalkWithInn_Desk", nil, nil, nil);
	
end

function onEventUpdate(player, npc, blah, menuSelect)
	
	player:EndEvent();
	
end