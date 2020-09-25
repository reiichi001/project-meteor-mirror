require ("global")

function onEventStarted(player, npc)
    defaultFst = GetStaticActor("DftFst");
	callClientFunction(player, "delegateEvent", player, defaultFst, "defaultTalkWithAUBRENARD_100", nil, nil, nil);
	player:endEvent();
end