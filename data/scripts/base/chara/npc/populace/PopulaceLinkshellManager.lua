function init(npc)
	return false, false, 0, 0;
end

function onEventStarted(player, npc)
	isNew = false;
	player:RunEventFunction("eventTalkStep1", isNew);
end

function onEventUpdate(player, npc, step, menuOptionSelected, lsName, lsCrest)

	if (menuOptionSelected == nil) then
		player:EndEvent();
		return;
	end

	isNew = false;
	if (menuOptionSelected == 1) then
		player:RunEventFunction("eventTalkStep2", isNew);
	elseif (menuOptionSelected == 10) then
		player:EndEvent();
		return;
	elseif (menuOptionSelected == 3) then
		--createLinkshell
		player:RunEventFunction("eventTalkStepMakeupDone", isNew);
	end	
	
end