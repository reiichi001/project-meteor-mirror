require("/quests/man/man0u0")

function onEventStarted(player, actor, triggerName)	

	man0u0Quest = getStaticActor("Man0u0");	
	player:runEventFunction("delegateEvent", player, man0u0Quest, "processTtrNomal001withHQ", nil, nil, nil, nil);
	
end

function onEventUpdate(player, npc, resultId)	

	player:endEvent();
	
end

function onTalked(player, npc)
	
	man0u0Quest = player:getQuest("Man0u0");
	
	if (man0u0Quest ~= nil) then
	
		
		
	end
	
end