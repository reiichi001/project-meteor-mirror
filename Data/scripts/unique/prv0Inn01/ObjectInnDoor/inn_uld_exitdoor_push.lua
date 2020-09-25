require ("global")

function onEventStarted(player, npc)
	defaultWil = GetStaticActor("DftWil");
	choice = callClientFunction(player, "delegateEvent", player, defaultWil, "defaultTalkWithInn_ExitDoor");
		
	if (choice == 1) then		
		GetWorldManager():DoZoneChange(player, 209, nil, 0, 15, -110.157, 202, 171.345, 0);
	end
	
	player:endEvent();
end