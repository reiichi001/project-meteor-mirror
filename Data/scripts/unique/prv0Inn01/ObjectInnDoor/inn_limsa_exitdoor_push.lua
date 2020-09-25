require ("global")

function onEventStarted(player, npc)
	defaultSea = GetStaticActor("DftSea");
	choice = callClientFunction(player, "delegateEvent", player, defaultSea, "defaultTalkWithInn_ExitDoor");
		
	if (choice == 1) then		
		GetWorldManager():DoZoneChange(player, 133, nil, 0, 15, -444.266, 39.518, 191, 1.9);
	end
	
	player:endEvent();
end