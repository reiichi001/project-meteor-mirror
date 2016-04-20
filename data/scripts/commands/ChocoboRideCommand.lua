--[[

ChocoboRideCommand Script

Handles mounting and dismounting the Chocobo and Goobbue

--]]

function onEventStarted(player, actor, triggerName, isGoobbue)

	if (player:getState() == 0) then		
				
		worldMaster = getWorldMaster();		
		
		if (isGoobbue ~= true) then
			player:changeMusic(83);
			player:sendChocoboAppearance();
			player:sendGameMessage(player, worldMaster, 26001, 0x20);
			player:setMountState(1);
		else
			player:changeMusic(98);
			player:sendGoobbueAppearance();
			player:sendGameMessage(player, worldMaster, 26019, 0x20);
			player:setMountState(2);
		end
		
		player:changeSpeed(0.0, 5.0, 10.0);
		player:changeState(15);
	else
		player:changeMusic(player:getZone().bgmDay);
		
		worldMaster = getWorldMaster();
		
		if (player:getMountState() == 1) then
			player:sendGameMessage(player, worldMaster, 26003, 0x20);
		else
			player:sendGameMessage(player, worldMaster, 26021, 0x20);		
		end
		
		player:setMountState(0);
		player:changeSpeed(0.0, 2.0, 5.0)
		player:changeState(0); 
	end
	
	player:endCommand();
	
end