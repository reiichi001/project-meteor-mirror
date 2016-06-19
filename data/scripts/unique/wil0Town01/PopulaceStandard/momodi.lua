require("global")

function onEventStarted(player, npc)
	defaultWil = GetStaticActor("DftWil");
	tutorialU1 = GetStaticActor("Trl0u1");
	--callClientFunction(player, "delegateEvent", player, defaultWil, "defaultTalkWithMomodi_001", nil, nil, nil);
	callClientFunction(player, "switchEvent", defaultWil, tutorialU1, nil, nil, 1, 1, 0x3F1);
	player:endEvent();
	
end

function onEventUpdate(player, npc, blah, menuSelect)
	
	
	
end