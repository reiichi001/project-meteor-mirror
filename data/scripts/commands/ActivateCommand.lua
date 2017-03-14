require ("global")

--[[

ActivateCommand Script

Switches between active and passive mode states

--]]

function onEventStarted(player, command, triggerName)	
	
	if (player:GetState() == 0) then
		player:ChangeState(2);
		sendSignal("playerActive");
	elseif (player:GetState() == 2) then
		player:ChangeState(0); 
		sendSignal("playerPassive");
	end
		
	player:endEvent();

end