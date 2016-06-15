
function onEventStarted(player, npc)
    defaultSea = GetStaticActor("DftSea");
    player:RunEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithMimiroon_001", nil, nil, nil);
end

