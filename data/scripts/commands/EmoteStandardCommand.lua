--[[

EmoteStandardCommand Script

--]]

emoteTable = {
{},
};


function onEventStarted(player, actor, triggerName, emoteId)

	if (player:GetState() == 0) then						
		player:DoEmote(emoteId);
	end
	
	player:EndEvent();
	
end

function onEventUpdate(player, npc)
end