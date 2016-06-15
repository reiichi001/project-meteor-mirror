--[[

ChocoboRideCommand Script

Handles mounting and dismounting the Chocobo and Goobbue

--]]

function onEventStarted(player, actor, triggerName, isGoobbue)

	if (player:GetState() == 0) then		
				
		worldMaster = GetWorldMaster();		
		
		if (isGoobbue ~= true) then
			player:changeMusic(83);
			player:SendChocoboAppearance();
			player:SendGameMessage(player, worldMaster, 26001, 0x20);
			player:SetMountState(1);
		else
			player:changeMusic(98);
			player:SendGoobbueAppearance();
			player:SendGameMessage(player, worldMaster, 26019, 0x20);
			player:SetMountState(2);
		end
		
		player:changeSpeed(0.0, 5.0, 10.0);
		player:changeState(15);
	else
		player:changeMusic(player:GetZone().bgmDay);
		
		worldMaster = GetWorldMaster();
		
		if (player:GetMountState() == 1) then
			player:SendGameMessage(player, worldMaster, 26003, 0x20);
		else
			player:SendGameMessage(player, worldMaster, 26021, 0x20);		
		end
		
		player:SetMountState(0);
		player:changeSpeed(0.0, 2.0, 5.0)
		player:changeState(0); 
	end
	
	player:EndCommand();
	
end