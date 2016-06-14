function onEventStarted(player, npc)
	defaultSea = getStaticActor("DftSea");
	player:runEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithInn_Desk", nil, nil, nil);
	
end

function onEventUpdate(player, npc, blah, menuSelect)

	if (menuSelect == 1) then
		getWorldManager():DoZoneChange(player, 12);
	end
	
	player:endEvent();
	
end