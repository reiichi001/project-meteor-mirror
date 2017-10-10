--[[

DiceCommand Script

--]]

function onEventStarted(player, actor, triggerName, maxNumber)	
	
	if (maxNumber == nil or maxNumber > 1000 or maxNumber < 1) then
		maxNumber = 100;
	end
		
	result = math.random(0, maxNumber);
	
	worldMaster = GetWorldMaster();
	player:SendGameMessage(player, worldMaster, 25342, 0x20, result, maxNumber);
				
	player:EndEvent();
	
end

function onEventUpdate(player, npc)
end