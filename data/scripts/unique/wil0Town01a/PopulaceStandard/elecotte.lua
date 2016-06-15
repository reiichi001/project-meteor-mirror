
function onEventStarted(player, npc)
    defaultWil = GetStaticActor("DftWil");
    player:RunEventFunction("delegateEvent", player, defaultWil, "defaultTalkWithElecotte_001", nil, nil, nil);
end

