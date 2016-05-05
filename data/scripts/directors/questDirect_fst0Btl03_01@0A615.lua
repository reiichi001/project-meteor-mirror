
function onEventStarted(player, actor, triggerName)	

	man0g0Quest = getStaticActor("Man0g0");
	--player:runEventFunction("delegateEvent", player, man0g0Quest, "processTtrBtl001");
	player:runEventFunction("delegateEvent", player, man0g0Quest, "processTtrBtl002");
	
end

function onEventUpdate(player, npc, resultId)	
	--man0g0Quest = getStaticActor("Man0g0");
	--player:runEventFunction("delegateEvent", player, man0g0Quest, "processTtrBtl002");
	player:endEvent();
end

function onCommand(player, command)
	--Check command if ActivateCommand
	player:endCommand();
	player:endEvent();
	player:kickEvent(player:getDirector(), "noticeEvent", true);	
end