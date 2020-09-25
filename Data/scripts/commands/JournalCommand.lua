require ("global")

--[[

JournalCommand Script

Fired when you try to abandon a quest

--]]

function onEventStarted(player, command, triggerName, questId)		
	
	player:AbandonQuest(questId);
	player:EndEvent();
	
end