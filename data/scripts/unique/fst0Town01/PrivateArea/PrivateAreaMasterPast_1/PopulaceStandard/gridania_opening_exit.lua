require ("global")
require ("quests/man/man0g0")

function onSpawn(player, npc)	
	npc:SetQuestGraphic(player, 0x3);	
end

function onEventStarted(player, npc)	
	man0g1Quest = GetStaticActor("Man0g1");		
	callClientFunction(player, "delegateEvent", player, man0g1Quest, "processEvent100");
	player:ReplaceQuest(110005, 110006);
	player:SendGameMessage(GetStaticActor("Man0g1"), 353, 0x20);
	player:SendGameMessage(GetStaticActor("Man0g1"), 354, 0x20);
	GetWorldManager():DoZoneChange(player, 155, "PrivateAreaMasterPast", 2, 15, 67.034, 4, -1205.6497, -1.074);	
	player:endEvent();
end