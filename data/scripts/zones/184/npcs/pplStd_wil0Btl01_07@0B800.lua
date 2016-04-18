require("/quests/man/man0u0")

function init(npc)
	return "/Chara/Npc/Populace/PopulaceStandard", false, false, false, false, false, npc.getActorClassId(), false, false, 0, 1, "TEST";	
end

function onEventStarted(player, npc, triggerName)

	man0u0Quest = getStaticActor("Man0u0");

	if (triggerName == "talkDefault") then
		player:runEventFunction("delegateEvent", player, man0u0Quest, "processEvent000_12", nil, nil, nil);
	else
		player:endEvent();
	end
	
end

function onEventUpdate(player, npc)	
	player:endEvent();
end