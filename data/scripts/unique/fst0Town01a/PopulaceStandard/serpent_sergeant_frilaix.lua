require ("global")

function onEventStarted(player, npc)
    defaultFst = GetStaticActor("Spl000");
	callClientFunction(player, "delegateEvent", player, defaultFst, "processEventARISMONT", 1, 1, 1);
	player:endEvent();
end