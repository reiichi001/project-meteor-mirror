
function onEventStarted(player, actor, triggerName)	

	man0l0Quest = getStaticActor("Man0l0");
	player:runEventFunction("delegateEvent", player, man0l0Quest, "processTtrBtl004");
	--player:runEventFunction("delegateEvent", player, man0l0Quest, "processTtrBtl002");
	
end

function onEventUpdate(player, npc, resultId)	
	--man0l0Quest = getStaticActor("Man0l0");
	--player:runEventFunction("delegateEvent", player, man0l0Quest, "processTtrBtl002");
	player:endEvent();
end

function onCommand(player, command)
	--Check command if ActivateCommand
	player:kickEvent(player:getDirector(), "noticeEvent", true);	
end