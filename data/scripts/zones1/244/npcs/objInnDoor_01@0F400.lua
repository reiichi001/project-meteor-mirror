function init(npc)
	return "/Chara/Npc/Object/ObjectInnDoor", false, false, false, false, false, npc.getActorClassId(), false, false, 0, 1, "TEST";	
end

function onEventStarted(player, npc, triggerName)
	defaultFst = getStaticActor("DftFst");
	player:runEventFunction("delegateEvent", player, defaultFst, "defaultTalkWithInn_ExitDoor", nil, nil, nil);
end

function onEventUpdate(player, npc, resultId, isExitYes)

	if (isExitYes ~= nil and isExitYes == 1) then
		getWorldManager():DoZoneChange(player, 1);
	else
		player:endEvent();
	end
	
end