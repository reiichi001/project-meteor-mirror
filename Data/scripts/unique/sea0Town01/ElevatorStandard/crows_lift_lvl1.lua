require ("global")

function onEventStarted(player, npc)		
	floorChoice = callClientFunction(player, "elevatorAskLimsa001", 0);	
	
	if (floorChoice == 1) then
		callClientFunction(player, "elevatorAskLimsa001", 1);
		GetWorldManager():DoZoneChange(player, 133, nil, 0, 15, -447, 40, 220, -1.574);	
	elseif (floorChoice == 2) then
		callClientFunction(player, "elevatorAskLimsa001", 2);	
		GetWorldManager():DoZoneChange(player, 133, nil, 0, 15, -458, 92, 175, -0.383);	
	end
	
	player:EndEvent();	
end