require ("global")

function onEventStarted(player, npc)
	defaultWil = GetStaticActor("DftWil");
	result = player:RunEventFunction("delegateEvent", player, defaultWil, "defaultTalkWithInn_Desk_2", nil, nil, nil); --BTN
	player:EndEvent();
end