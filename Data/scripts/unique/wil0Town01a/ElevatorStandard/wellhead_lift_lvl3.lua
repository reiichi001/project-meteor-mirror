require ("global")

function onEventStarted(player, npc)		
	floorChoice = callClientFunction(player, "elevatorAskUldah003", 0);	
	
	if (floorChoice == 1) then
		callClientFunction(player, "elevatorAskUldah003", 1);		
		GetWorldManager():DoZoneChange(player, 175, nil, 0, 15, -116.78, 198, 115.7, -2.8911);
	elseif (floorChoice == 2) then
		callClientFunction(player, "elevatorAskUldah003", 2);
		GetWorldManager():DoZoneChange(player, 209, nil, 0, 15, -116.78, 222, 115.7, 2.85);
	end
	
	player:EndEvent();	
end