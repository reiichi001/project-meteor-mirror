require("/quests/man/man0l0")

function onEventStarted(player, actor, triggerName)	
	
	man0l0Quest = getStaticActor("Man0l0");
	player:runEventFunction("delegateEvent", player, man0l0Quest, "processTtrNomal001withHQ", nil, nil, nil, nil);
	--player:runEventFunction("delegateEvent", player, man0l0Quest, "processEvent000_1", nil, nil, nil, nil);
	
end

function onEventUpdate(player, npc, resultId)
	man0l0Quest = getStaticActor("Man0l0");
	
	if (resultId == RESULT_Event000_1) then
		player:runEventFunction("delegateEvent", player, man0l0Quest, "processTtrNomal001", nil, nil, nil, nil);
	elseif (resultId == RESULT_TtrNomal001) then
		player:endEvent();
	end
	
end