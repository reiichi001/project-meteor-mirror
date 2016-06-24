require ("global")

function onEventStarted(player, npc)		
	floorChoice = callClientFunction(player, "elevatorAskLimsa003", 0);	
	
	if (floorChoice == 1) then
		callClientFunction(player, "elevatorAskLimsa003", 1);		
	elseif (floorChoice == 2) then
		callClientFunction(player, "elevatorAskLimsa003", 2);
	end
	
	player:EndEvent();	
end