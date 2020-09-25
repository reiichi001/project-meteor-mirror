require ("global")

--Argument is 20 or ~20.

function onEventStarted(player, npc)
	defaultSea = GetStaticActor("DftSea");
	callClientFunction(player, "delegateEvent", player, defaultSea, "defaultTalkWithSolelle_001", 0);
	player:endEvent();
end