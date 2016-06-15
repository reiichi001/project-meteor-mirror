function onEventStarted(player, npc)
	defaultWil = GetStaticActor("DftWil");
	player:RunEventFunction("delegateEvent", player, defaultWil, "defaultTalkWithInn_Desk_2", nil, nil, nil); --BTN
	
end

function onEventUpdate(player, npc, blah, menuSelect)

	if (menuSelect == 1) then
		GetWorldManager():DoZoneChange(player, 11);		
	end
	
	player:EndEvent();
	
end