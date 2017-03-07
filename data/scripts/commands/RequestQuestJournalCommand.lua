--[[


--]]

function onEventStarted(player, actor, trigger, questId, mapCode)

	quest = player:GetQuest(questId);
	if (mapCode == nil) then	
		player:SendRequestedInfo("requestedData", "qtdata", quest:GetQuestId(), 3);
		player:EndEvent();
	else
		player:SendRequestedInfo("requestedData", "qtmap", quest:GetQuestId());
		player:EndEvent();
	end
	
end
