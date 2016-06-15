
function onEventStarted(player, npc)
    defaultSea = GetStaticActor("DftSea");
    player:RunEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithEugennoix_001", nil, nil, nil);
end

