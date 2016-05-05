
function onEventStarted(player, actor, triggerName)	

	man0u0Quest = getStaticActor("Man0u0");
	man0l0Quest = getStaticActor("Man0l0");
	--player:runEventFunction("delegateEvent", player, man0l0Quest, "processTtrBtl001");
	--player:runEventFunction("delegateEvent", player, man0l0Quest, "processTtrBtlMagic001");
	--player:runEventFunction("delegateEvent", player, man0l0Quest, "processTtrBtl002");
	--player:runEventFunction("delegateEvent", player, man0l0Quest, "processTtrBtl003");
	
	player:runEventFunction("delegateEvent", player, man0u0Quest, "processTtrBtl004");
	
end

function onEventUpdate(player, npc, resultId)	
	--man0l0Quest = getStaticActor("Man0l0");
	--player:runEventFunction("delegateEvent", player, man0l0Quest, "processTtrBtl002");
	player:endEvent();
end

function onCommand(player, command)
	--Check command if ActivateCommand
	--player:kickEvent(player:getDirector(), "noticeEvent");	
	--player:endCommand();
end