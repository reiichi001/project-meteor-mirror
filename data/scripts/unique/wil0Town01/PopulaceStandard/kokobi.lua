
function onEventStarted(player, npc)
    defaultWil = GetStaticActor("DftWil");
    player:RunEventFunction("delegateEvent", player, defaultWil, "downTownTalk", nil, nil, nil);
end

