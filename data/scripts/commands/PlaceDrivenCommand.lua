--[[

PlaceDrivenCommand Script

Notes: 


--]]

function onEventStarted(player, commandActor, triggerName, pushCommand, unk1, unk2, unk3, ownerActorId, unk4, unk5, unk6, unk7)
	
	actor = player:GetActorInInstance(ownerActorId);
	
	if (actor != nil) then
		player:kickEvent(actor, "pushCommand", "pushCommand");
	else
		player:endEvent();
	end
	
end