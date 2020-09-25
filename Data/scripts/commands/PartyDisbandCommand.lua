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

function onEventStarted(player, actor, name)
	worldMaster = GetWorldMaster();		
	
	if (player:IsPartyLeader()) then
		player:PartyKickPlayer(name);
	else
		player:SendGameMessage(player, worldMaster, 30540, 0x20);
	end
	
	player:EndEvent();
end