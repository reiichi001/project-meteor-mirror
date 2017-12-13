require ("global")

function onEventStarted(player, npc)
    Spl = GetStaticActor("Spl000");
	callClientFunction(player, "delegateEvent", player, Spl, "processEventSISIMUZA");
	player:endEvent();
end