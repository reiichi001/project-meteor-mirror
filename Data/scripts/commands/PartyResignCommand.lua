--[[

PartyResignCommand Script

Handles leaving a party

--]]

function onEventStarted(player, actor, triggerName)
	player:PartyLeave(name);	
	player:EndEvent();
end