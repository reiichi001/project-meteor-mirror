require ("global")

function onEventStarted(player, npc)
    defaultFst = GetStaticActor("Spl000");
	callClientFunction(player, "delegateEvent", player, defaultFst, "processEventMERLIE", 1,1,1);
	player:endEvent();
end