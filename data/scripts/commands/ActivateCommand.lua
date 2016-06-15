--[[

ActivateCommand Script

Switches between active and passive mode states

--]]

function onEventStarted(player, command, triggerName)	
	
	if (player:GetState() == 0) then
		player:changeState(2);
	elseif (player:GetState() == 2) then
		player:changeState(0); 
	end
		
	player:EndCommand();
	
	--For Opening Tutorial
	if (player:hasQuest("Man0l0") or player:hasQuest("Man0g0") or player:hasQuest("Man0u0")) then
		player:GetDirector():OnCommand(command);
	
	end	
	
end