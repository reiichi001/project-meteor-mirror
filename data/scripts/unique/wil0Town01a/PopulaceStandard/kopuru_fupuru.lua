function onEventStarted(player, npc)
	defaultWil = getStaticActor("DftWil");
	player:runEventFunction("delegateEvent", player, defaultWil, "defaultTalkWithInn_Desk_2", nil, nil, nil); --BTN
	
end

function onEventUpdate(player, npc, blah, menuSelect)

	if (menuSelect == 1) then
		getWorldManager():DoZoneChange(player, 11);		
	end
	
	player:endEvent();
	
end