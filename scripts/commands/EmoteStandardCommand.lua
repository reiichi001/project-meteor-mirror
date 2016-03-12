--[[

EmoteStandardCommand Script

--]]

emoteTable = {
{},
};


function onEventStarted(player, actor, emoteId)

	if (player:getState() == 0) then						
		player:doEmote(emoteId);
	end
	
	player:endEvent();
	
end

function onEventUpdate(player, npc)
end