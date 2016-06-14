function onEventStarted(player, npc)
	defaultFst = getStaticActor("DftFst");
	player:runEventFunction("delegateEvent", player, defaultFst, "defaultTalkWithInn_Desk", nil, nil, nil);
	
end

function onEventUpdate(player, npc, blah, menuSelect)

	if (menuSelect == 1) then
		getWorldManager():DoZoneChange(player, 13);
	end
	
	player:endEvent();
	
end