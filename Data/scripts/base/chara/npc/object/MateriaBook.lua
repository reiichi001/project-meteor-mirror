require ("global")

function init(npc)
	return false, false, 0, 0;	
end

function onEventStarted(player, npc, triggerName)
	callClientFunction(player, "materiabookTalk");
	player:EndEvent();
end