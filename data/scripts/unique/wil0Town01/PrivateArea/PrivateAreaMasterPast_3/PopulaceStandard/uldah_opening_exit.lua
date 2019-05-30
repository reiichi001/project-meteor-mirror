require ("global")
require ("quests/man/man0u0")

function onSpawn(player, npc)	
	npc:SetQuestGraphic(player, 0x3);	
end

function onEventStarted(player, npc)	
	man0u1Quest = GetStaticActor("Man0u1");		
	callClientFunction(player, "delegateEvent", player, man0u1Quest, "processEventMomodiStart");
	player:ReplaceQuest(110009, 110010);
	player:SendGameMessage(GetStaticActor("Man0u1"), 329, 0x20);
	player:SendGameMessage(GetStaticActor("Man0u1"), 330, 0x20);
	GetWorldManager():DoZoneChange(player, 175, "PrivateAreaMasterPast", 4, 15, -75.242, 195.009, 74.572, -0.046);	
	player:endEvent();
end