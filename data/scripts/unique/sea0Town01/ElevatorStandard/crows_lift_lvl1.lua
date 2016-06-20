require ("global")

function onEventStarted(player, npc)		
	floorChoice = callClientFunction(player, "elevatorAskLimsa001", 0);	
	
	if (floorChoice == 1) then
		callClientFunction(player, "elevatorAskLimsa001", 1);				
	elseif (floorChoice == 2) then
		callClientFunction(player, "elevatorAskLimsa001", 2);		
	end
	
	player:EndEvent();	
end