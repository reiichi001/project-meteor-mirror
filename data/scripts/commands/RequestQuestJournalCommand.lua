--[[


--]]

function onEventStarted(player, actor, trigger, questId, mapCode)

	quest = player:GetQuest(questId);
	
	if (quest == nil) then	
		player:EndEvent();
		return;
	end
	
	if (mapCode == nil) then	
		player:SendRequestedInfo("requestedData", "qtdata", quest:GetQuestId(), quest:GetPhase());
		player:EndEvent();
	else
		player:SendRequestedInfo("requestedData", "qtmap", quest:GetQuestId());
		player:EndEvent();
	end
	
end
