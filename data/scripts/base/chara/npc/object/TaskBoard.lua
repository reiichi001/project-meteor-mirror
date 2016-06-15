function init(npc)
	return false, false, 0, 0;	
end

function onEventStarted(player, npc, triggerName)
	questNOC = GetStaticActor("Noc000");
	
	if (npc:GetActorClassId() == 1200193) then
		player:RunEventFunction("delegateEvent", player, questNOC, "pETaskBoardAskLimsa", nil, nil, nil);
	elseif (npc:GetActorClassId() == 1200194) then
		player:RunEventFunction("delegateEvent", player, questNOC, "pETaskBoardAskUldah", nil, nil, nil);
	else
		player:RunEventFunction("delegateEvent", player, questNOC, "pETaskBoardAskGridania", nil, nil, nil);
	end
	
end

function onEventUpdate(player, npc, step, menuOptionSelected)
	player:EndEvent();	
end