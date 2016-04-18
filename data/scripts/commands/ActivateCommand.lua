--[[

ActivateCommand Script

Switches between active and passive mode states

--]]

function onEventStarted(player, command, triggerName)	
	
	if (player:getState() == 0) then
		player:changeState(2);
	elseif (player:getState() == 2) then
		player:changeState(0); 
	end
		
	player:endCommand();
	
	--For Opening Tutorial
	if (player:hasQuest("Man0l0") or player:hasQuest("Man0g0") or player:hasQuest("Man0u0")) then
		player:getDirector():onCommand(command);
	
	end	
	
end