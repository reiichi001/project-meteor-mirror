--[[

EmoteStandardCommand Script

--]]

emoteTable = {
{},
};


function onEventStarted(player, actor, triggerName, emoteId)

	if (player:GetState() == 0) then						
		player:doEmote(emoteId);
	end
	
	player:EndCommand();
	
end

function onEventUpdate(player, npc)
end