require ("global")
require ("quests/man/man0l0")

function onEventStarted(player, npc)
	man0l0Quest = player:GetQuest("Man0l0");
	choice = callClientFunction(player, "delegateEvent", player, man0l0Quest, "processEvent020_9", nil, nil, nil);
		
	if (choice == 1) then
		man0l1Quest = GetStaticActor("Man0l1");		
		callClientFunction(player, "delegateEvent", player, man0l1Quest, "processEvent010", nil, nil, nil);
		player:ReplaceQuest(110001, 110002);
		player:SendGameMessage(GetStaticActor("Man0l1"), 320, 0x20);
		player:SendGameMessage(GetStaticActor("Man0l1"), 321, 0x20);
		GetWorldManager():DoZoneChange(player, 133, "PrivateAreaMasterPast", 2, 15, -459.619873, 40.0005722, 196.370377, 2.010813);
	end
	
	player:endEvent();
end