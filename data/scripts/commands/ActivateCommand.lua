require ("global")

--[[

ActivateCommand Script

Switches between active and passive mode states

--]]

function onEventStarted(player, command, triggerName)		
	
	if (player:GetState() == 0) then
		player.Engage();
	elseif (player:GetState() == 2) then
		player:ChangeState(0);
        player.Disengage();
	end		
	player:endEvent();
	sendSignal("playerActive");
	
end