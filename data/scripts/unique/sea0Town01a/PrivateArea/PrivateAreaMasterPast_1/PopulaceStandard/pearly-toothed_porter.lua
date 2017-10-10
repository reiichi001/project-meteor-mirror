require ("global")
require ("quests/man/man0l0")

function onEventStarted(player, npc)
	man0l0Quest = player:GetQuest("Man0l0");
	callClientFunction(player, "delegateEvent", player, man0l0Quest, "processEvent020_3", nil, nil, nil);
	player:endEvent();
end