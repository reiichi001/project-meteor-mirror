
function onEventStarted(player, actor, triggerName)	

	man0g0Quest = GetStaticActor("Man0g0");
	--player:RunEventFunction("delegateEvent", player, man0g0Quest, "processTtrBtl001");
	player:RunEventFunction("delegateEvent", player, man0g0Quest, "processTtrBtl002");
	
end

function onEventUpdate(player, npc, resultId)	
	--man0g0Quest = GetStaticActor("Man0g0");
	--player:RunEventFunction("delegateEvent", player, man0g0Quest, "processTtrBtl002");
	player:EndEvent();
end

function onCommand(player, command)
	--Check command if ActivateCommand
	player:EndCommand();
	player:EndEvent();
	player:KickEvent(player:GetDirector(), "noticeEvent", true);	
end