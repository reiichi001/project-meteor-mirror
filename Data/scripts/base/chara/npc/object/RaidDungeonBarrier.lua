require ("global")

function init(npc)
	return false, false, 0, 0;	
end

function onEventStarted(player, npc, triggerName)

	isActive = true;

	if (isActive) then
		choice = callClientFunction(player, "askYesNo");
	else
		callClientFunction(player, "eventTalkRead");
	end
	
	player:EndEvent();
	
end