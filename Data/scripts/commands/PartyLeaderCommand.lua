--[[

PartyLeaderCommand Script

Handles requesting to change party leader and various errors.

--]]

function onEventStarted(player, actor, triggerName, name, arg2, arg3, arg4, actorId)
	worldMaster = GetWorldMaster();		
	
	if (player:IsPartyLeader()) then
		if (name == nil) then
			player:PartyPromote(actorId);
		else
			player:PartyPromote(name);
		end
	else
		player:SendGameMessage(player, worldMaster, 30540, 0x20);
	end
	
	player:EndEvent();
end