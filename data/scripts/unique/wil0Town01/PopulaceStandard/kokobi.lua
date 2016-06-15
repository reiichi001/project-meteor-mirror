
function onEventStarted(player, npc)
    defaultWil = getStaticActor("DftWil");
    player:runEventFunction("delegateEvent", player, defaultWil, "downTownTalk", nil, nil, nil);
end

