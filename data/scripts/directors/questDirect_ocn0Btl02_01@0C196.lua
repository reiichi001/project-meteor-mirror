
function onEventStarted(player, actor, triggerName)	

	man0u0Quest = GetStaticActor("Man0u0");
	man0l0Quest = GetStaticActor("Man0l0");
	player:RunEventFunction("delegateEvent", player, man0l0Quest, "processTtrBtl001");
	--player:RunEventFunction("delegateEvent", player, man0l0Quest, "processTtrBtlMagic001");
	--player:RunEventFunction("delegateEvent", player, man0l0Quest, "processTtrBtl002");
	--player:RunEventFunction("delegateEvent", player, man0l0Quest, "processTtrBtl003");
	
	--player:RunEventFunction("delegateEvent", player, man0u0Quest, "processTtrBtl004");
	
end

function onEventUpdate(player, npc, resultId)	
	--man0l0Quest = GetStaticActor("Man0l0");
	--player:RunEventFunction("delegateEvent", player, man0l0Quest, "processTtrBtl002");
	player:EndEvent();
end

function onCommand(player, command)
	--Check command if ActivateCommand
	--player:KickEvent(player:GetDirector(), "noticeEvent");	
	--player:EndCommand();
end