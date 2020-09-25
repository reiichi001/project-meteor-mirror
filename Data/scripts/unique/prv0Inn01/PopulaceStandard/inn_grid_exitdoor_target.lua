require ("global")

function onEventStarted(player, npc)
	defaultFst = GetStaticActor("DftFst");
	choice = callClientFunction(player, "delegateEvent", player, defaultFst, "defaultTalkWithInn_ExitDoor");
		
	if (choice == 1) then		
		GetWorldManager():DoZoneChange(player, 155, nil, 0, 15, 59.252, 4, -1219.342, 0.852);
	end
	
	player:endEvent();
end