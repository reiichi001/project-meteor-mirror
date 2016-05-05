function init(npc)
	return "/Chara/Npc/Populace/PopulaceLinkshellManager", false, false, false, false, false, npc.getActorClassId(), false, false, 0, 1, "TEST";	
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