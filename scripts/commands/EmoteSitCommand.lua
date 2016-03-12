--[[

EmoteSitCommand Script

--]]

function onEventStarted(player, actor, emoteId)

	if (player:getState() == 0) then						
		if (emoteId == 0x2712) then
			player:changeState(11);
		else
			player:changeState(13);
		end
	else
		player:changeState(0);
	end
	
	player:endEvent();
	
end

function onEventUpdate(player, npc)
end