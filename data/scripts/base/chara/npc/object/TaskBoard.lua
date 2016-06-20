require ("global")

function init(npc)
	return false, false, 0, 0;	
end

function onEventStarted(player, npc, triggerName)
	questNOC = GetStaticActor("Noc000");
	
	if (npc:GetActorClassId() == 1200193) then
		callClientFunction(player, "delegateEvent", player, questNOC, "pETaskBoardAskLimsa", nil, nil, nil);
	elseif (npc:GetActorClassId() == 1200194) then
		callClientFunction(player, "delegateEvent", player, questNOC, "pETaskBoardAskUldah", nil, nil, nil);
	else
		callClientFunction(player, "delegateEvent", player, questNOC, "pETaskBoardAskGridania", nil, nil, nil);
	end	
	
	player:EndEvent();	
end