
function init(npc)
	return false, false, 0, 0;	
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