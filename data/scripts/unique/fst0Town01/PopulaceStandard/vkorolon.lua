function onEventStarted(player, npc)
	defaultFst = GetStaticActor("DftFst");
	player:RunEventFunction("delegateEvent", player, defaultFst, "defaultTalkWithInn_Desk", nil, nil, nil);
	
end

function onEventUpdate(player, npc, blah, menuSelect)

	if (menuSelect == 1) then
		GetWorldManager():DoZoneChange(player, 13);
	end
	
	player:EndEvent();
	
end