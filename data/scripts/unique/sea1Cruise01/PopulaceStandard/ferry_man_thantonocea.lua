require ("global")

function onEventStarted(player, npc, triggerName)
	defaultSrt = GetStaticActor("DftSrt");
	callClientFunction(player, "delegateEvent", player, defaultSrt, "defaultTalkWithPilot_001");
	player:endEvent();
end