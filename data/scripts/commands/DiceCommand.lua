--[[

DiceCommand Script

--]]

function onEventStarted(player, actor, triggerName, maxNumber)	
	
	if (maxNumber == nil) then
		maxNumber = 999;
	end
		
	result = math.random(0, maxNumber);
	
	worldMaster = GetWorldMaster();
	player:SendGameMessage(player, worldMaster, 25342, 0x20, result, maxNumber);
				
	player:EndCommand();
	
end

function onEventUpdate(player, npc)
end