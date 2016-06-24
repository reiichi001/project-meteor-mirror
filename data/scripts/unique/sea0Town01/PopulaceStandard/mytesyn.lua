require ("global")

function onEventStarted(player, npc)
	defaultSea = GetStaticActor("DftSea");
	choice = callClientFunction(player, "delegateEvent", player, defaultSea, "defaultTalkWithInn_Desk", nil, nil, nil);
	
	if (choice == 1) then
		GetWorldManager():DoZoneChange(player, 13);
	elseif (choice == 2) then
		--Do Set Homepoint
	end
	
	player:EndEvent();	
end