function onInstantiate(npc)
	return "/Chara/Npc/Populace/PopulaceTutorial", false, false, false, false, false, npc.getActorClassId(), false, false, 0, 1, "TEST";	
end

function onEventStarted(player, npc, triggerName)

	man0l0Quest = getStaticActor("Man0l0");

	if (triggerName == "pushDefault") then
		player:runEventFunction("delegateEvent", player, man0l0Quest, "processTtrNomal002", nil, nil, nil);
		player:setEventStatus(npc, "pushDefault", false, 0x2);
	elseif (triggerName == "talkDefault") then
		--if () then
		--	player:runEventFunction("delegateEvent", player, man0l0Quest, "processTtrNomal003", nil, nil, nil);
		--else
			player:runEventFunction("delegateEvent", player, man0l0Quest, "processTtrMini001", nil, nil, nil);
		--end
	else
		player:endEvent();
	end
	
end

function onEventUpdate(player, npc)	
	player:endEvent();
end