
function onEventStarted(player, actor, triggerName)	

	man0l0Quest = getStaticActor("Man0l0");
	--player:runEventFunction("delegateEvent", player, man0l0Quest, "processTtrNomal001withHQ", nil, nil, nil, nil);
	player:runEventFunction("delegateEvent", player, man0l0Quest, "processEvent000_1", nil, nil, nil, nil);
	
	
end

function onEventUpdate(player, npc ,triggerName)	
	player:endEvent();
end