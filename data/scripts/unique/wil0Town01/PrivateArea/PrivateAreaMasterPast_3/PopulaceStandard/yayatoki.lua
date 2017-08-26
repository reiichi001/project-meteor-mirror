require ("global")
require ("quests/man/man0u0")

function onSpawn(player, npc)	
	npc:SetQuestGraphic(player, 0x2);	
end

function onEventStarted(player, npc, triggerName)
	man0u0Quest = GetStaticActor("Man0u0");	
	callClientFunction(player, "delegateEvent", player, man0u0Quest, "processEvent020_8");	
	npc:SetQuestGraphic(player, 0x0);
	player:EndEvent();
end