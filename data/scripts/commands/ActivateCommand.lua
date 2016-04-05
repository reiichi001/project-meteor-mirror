--[[

ActivateCommand Script

Switches between active and passive mode states

--]]

function onEventStarted(player, actor, triggerName)	

	if (player:getState() == 0) then
		player:changeState(2);
	elseif (player:getState() == 2) then
		player:changeState(0); 
	end
	
	player:endEvent();
	
end

function onEventUpdate(player, npc)
end