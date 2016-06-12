function init(npc)
	return false, false, 0, 0;
end

function onEventStarted(player, npc)
	isNew = false;
	player:runEventFunction("eventTalkStep1", isNew);
end

function onEventUpdate(player, npc, step, menuOptionSelected, lsName, lsCrest)

	if (menuOptionSelected == nil) then
		player:endEvent();
		return;
	end

	isNew = false;
	if (menuOptionSelected == 1) then
		player:runEventFunction("eventTalkStep2", isNew);
	elseif (menuOptionSelected == 10) then
		player:endEvent();
		return;
	elseif (menuOptionSelected == 3) then
		--createLinkshell
		player:runEventFunction("eventTalkStepMakeupDone", isNew);
	end	
	
end