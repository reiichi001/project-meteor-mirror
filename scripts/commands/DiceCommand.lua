--[[

DiceCommand Script

--]]

function onEventStarted(player, actor, maxNumber)	
	
	if (maxNumber == nil) then
		maxNumber = 999;
	end
		
	result = math.random(0, maxNumber);
	
	worldMaster = getWorldMaster();
	player:sendGameMessage(player, worldMaster, 25342, 0x20, result, maxNumber);
				
	player:endEvent();
	
end

function onEventUpdate(player, npc)
end