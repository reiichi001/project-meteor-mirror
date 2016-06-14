function init(npc)
	return false, false, 0, 0;	
end

function onEventStarted(player, npc, triggerName)
	questNOC = getStaticActor("Noc000");
	
	if (npc:getActorClassId() == 1200193) then
		player:runEventFunction("delegateEvent", player, questNOC, "pETaskBoardAskLimsa", nil, nil, nil);
	elseif (npc:getActorClassId() == 1200194) then
		player:runEventFunction("delegateEvent", player, questNOC, "pETaskBoardAskUldah", nil, nil, nil);
	else
		player:runEventFunction("delegateEvent", player, questNOC, "pETaskBoardAskGridania", nil, nil, nil);
	end
	
end

function onEventUpdate(player, npc, step, menuOptionSelected)
	player:endEvent();	
end