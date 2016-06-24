require ("global")

function onEventStarted(player, npc)		
	floorChoice = callClientFunction(player, "elevatorAskLimsa002", 0);	
	
	if (floorChoice == 1) then
		callClientFunction(player, "elevatorAskLimsa002", 1);		
	elseif (floorChoice == 2) then
		callClientFunction(player, "elevatorAskLimsa002", 2);
	end
	
	player:EndEvent();	
end