require ("global")

function onEventStarted(player, npc)
    defaultFst = GetStaticActor("DftFst");
	callClientFunction(player, "delegateEvent", player, defaultFst, "defaultTalkWithAubrenard (check cnstctr)_001", nil, nil, nil);
	player:endEvent();
end