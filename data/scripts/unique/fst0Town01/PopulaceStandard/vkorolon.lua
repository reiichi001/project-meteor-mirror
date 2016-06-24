require ("global")

function onEventStarted(player, npc)
	defaultFst = GetStaticActor("DftFst");
	choice = callClientFunction(player, "delegateEvent", player, defaultFst, "defaultTalkWithInn_Desk", nil, nil, nil);
	
	if (choice == 1) then
		GetWorldManager():DoZoneChange(player, 13);
	elseif (choice == 2) then
		--Do Set Homepoint
	end
	
	player:EndEvent();
	
end