--[[

PartyKickCommand Script

Handles requesting to kick (oust) and various errors.

TextIds:

30404 - Ousted Sheet/ActorId Version
30410 - You are Ousted
30428 - Ousted String Version
30540 - You are not party leader
30555 - Unable to oust
30575 - Cannot oust due to not pt member

--]]

function onEventStarted(player, actor, triggerName, name, arg2, arg3, arg4, actorId)
	worldMaster = GetWorldMaster();		
	
	if (player:IsPartyLeader()) then
		if (name == nil) then
			player:PartyOustPlayer(actorId);
		else
			player:PartyOustPlayer(name);
		end
	else
		player:SendGameMessage(player, worldMaster, 30540, 0x20);
	end
	
	player:EndEvent();
end