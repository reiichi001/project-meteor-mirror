--[[

PartyBreakupCommand Script

Handles disbanding the party.

--]]

function onEventStarted(player, actor, triggerName)
	worldMaster = GetWorldMaster();		
	
	if (player:IsPartyLeader()) then
		player:PartyDisband(name)
	else
		player:SendGameMessage(player, worldMaster, 30540, 0x20);
	end
	
	player:EndEvent();
end