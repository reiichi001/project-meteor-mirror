require ("global")

function onEventStarted(player, npc)		
	floorChoice = callClientFunction(player, "elevatorAskUldah001", 0);	
	
	if (floorChoice == 1) then
		callClientFunction(player, "elevatorAskUldah001", 1);
		GetWorldManager():DoZoneChange(player, 209, nil, 0, 15, -116.78, 222, 115.7, 2.85);
	elseif (floorChoice == 2) then
		callClientFunction(player, "elevatorAskUldah001", 2);
		GetWorldManager():DoZoneChange(player, 209, nil, 0, 15, -121.60, 269.8, 135.28, -0.268);
	end
	
	player:EndEvent();	
end