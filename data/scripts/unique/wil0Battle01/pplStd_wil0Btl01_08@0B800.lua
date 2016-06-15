require("/quests/man/man0u0")

function init(npc)
	return "/Chara/Npc/Populace/PopulaceStandard", false, false, false, false, false, npc.GetActorClassId(), false, false, 0, 1, "TEST";	
end

function onEventStarted(player, npc, triggerName)

	man0u0Quest = GetStaticActor("Man0u0");

	if (triggerName == "talkDefault") then
		player:RunEventFunction("delegateEvent", player, man0u0Quest, "processEvent000_13", nil, nil, nil);
	else
		player:EndEvent();
	end
	
end

function onEventUpdate(player, npc)	
	player:EndEvent();
end