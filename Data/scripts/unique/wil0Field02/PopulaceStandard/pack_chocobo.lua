require ("global")

function onEventStarted(player, npc)
    defaultWil = GetStaticActor("DftWil");
	callClientFunction(player, "delegateEvent", player, defaultWil, "defaultTalkCaravanChocoboUld_001", nil, nil, nil);
	player:endEvent();
end