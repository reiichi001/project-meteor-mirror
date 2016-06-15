function onEventStarted(player, npc)
	defaultSea = GetStaticActor("DftSea");
	player:RunEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithInn_Desk", nil, nil, nil);
	
end

function onEventUpdate(player, npc, blah, menuSelect)

	if (menuSelect == 1) then
		GetWorldManager():DoZoneChange(player, 12);
	end
	
	player:EndEvent();
	
end