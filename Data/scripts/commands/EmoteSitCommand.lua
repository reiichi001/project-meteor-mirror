--[[

EmoteSitCommand Script

--]]

function onEventStarted(player, actor, triggerName, emoteId)

	if (player:GetState() == 0) then						
		if (emoteId == 0x2712) then
			player:ChangeState(11);
		else
			player:ChangeState(13);
		end
	else
		player:ChangeState(0);
	end
	
	player:EndEvent();
	
end

function onEventUpdate(player, npc)
end