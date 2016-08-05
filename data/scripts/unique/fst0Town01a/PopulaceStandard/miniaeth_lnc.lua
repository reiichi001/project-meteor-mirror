require ("global")

function onEventStarted(player, npc)
    defaultFst = GetStaticActor("DftFst");
	callClientFunction(player, "delegateEvent", player, defaultFst, "defaultTalkWithMiniaeth_lnc_001", nil, nil, nil);
	player:endEvent();
end