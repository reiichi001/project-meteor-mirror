require("/quests/man/man0u0")

function onEventStarted(player, actor, triggerName)	

	man0u0Quest = GetStaticActor("Man0u0");	
	player:RunEventFunction("delegateEvent", player, man0u0Quest, "processTtrNomal001withHQ", nil, nil, nil, nil);
	
end

function onEventUpdate(player, npc, resultId)	

	player:EndEvent();
	
end

function onTalked(player, npc)
	
	man0u0Quest = player:GetQuest("Man0u0");
	
	if (man0u0Quest ~= nil) then
	
		
		
	end
	
end