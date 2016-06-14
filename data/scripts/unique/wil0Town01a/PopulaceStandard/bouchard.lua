
function onEventStarted(player, npc)
    defaultWil = getStaticActor("DftWil");
    player:runEventFunction("delegateEvent", player, defaultWil, "defaultTalkWithBouchard_001", nil, nil, nil);
end

