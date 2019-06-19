require ("global")

function onEventStarted(player, npc)		
	floorChoice = callClientFunction(player, "elevatorAskLimsa003", 0);	
	
	if (floorChoice == 1) then
		callClientFunction(player, "elevatorAskLimsa003", 1);
		GetWorldManager():DoZoneChange(player, 133, nil, 0, 15, -447, 19, 220, -1.574);
	elseif (floorChoice == 2) then
		callClientFunction(player, "elevatorAskLimsa003", 2);
		GetWorldManager():DoZoneChange(player, 133, nil, 0, 15, -447, 40, 220, -1.574);
	end
	
	player:EndEvent();	
end