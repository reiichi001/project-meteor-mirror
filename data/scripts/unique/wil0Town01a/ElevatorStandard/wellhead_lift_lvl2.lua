require ("global")

function onEventStarted(player, npc)		
	floorChoice = callClientFunction(player, "elevatorAskUldah002", 0);	
	
	if (floorChoice == 1) then
		callClientFunction(player, "elevatorAskUldah002", 1);
		GetWorldManager():DoZoneChange(player, 175, nil, 0, 15, -116.78, 198, 115.7, -2.8911);
	elseif (floorChoice == 2) then
		callClientFunction(player, "elevatorAskUldah002", 2);
		GetWorldManager():DoZoneChange(player, 209, nil, 0, 15, -121.60, 269.8, 135.28, -0.268);
	end
	
	player:EndEvent();	
end