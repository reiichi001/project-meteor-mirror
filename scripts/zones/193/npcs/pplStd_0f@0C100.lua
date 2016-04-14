require("/quests/man/man0l0")

function init(npc)
	return "/Chara/Npc/Populace/PopulaceTutorial", false, false, false, false, false, npc.getActorClassId(), false, false, 0, 1, "TEST";	
end

function onSpawn(player, npc)

	man0l0Quest = player:getQuest("Man0l0");
	
	if (man0l0Quest ~= nil) then
		if (man0l0Quest ~= nil and man0l0Quest:GetQuestFlag(MAN0L0_FLAG_MINITUT_DONE1) == true and man0l0Quest:GetQuestFlag(MAN0L0_FLAG_MINITUT_DONE2) == true and man0l0Quest:GetQuestFlag(MAN0L0_FLAG_MINITUT_DONE3) == true) then
			player:setEventStatus(npc, "pushDefault", true, 0x2);
			npc:setQuestGraphic(player, 0x3);
		else
			player:setEventStatus(npc, "pushDefault", true, 0x2);
			npc:setQuestGraphic(player, 0x3);
		end
	end
	
end

function onEventStarted(player, npc, triggerName)

	if (triggerName == "pushDefault") then
		man0l0Quest = getStaticActor("Man0l0");
		player:runEventFunction("delegateEvent", player, man0l0Quest, "processEventNewRectAsk", nil);
	else
		player:endEvent();
	end
	
end

function onEventUpdate(player, npc, resultId, choice)

	if (resultId == 0x2B9EBC42) then
		player:endEvent();
		player:setDirector("QuestDirectorMan0l001", true);
		
		worldMaster = getWorldMaster();
		player:sendGameMessage(player, worldMaster, 34108, 0x20);	
		player:sendGameMessage(player, worldMaster, 50011, 0x20);	

		getWorldManager():DoPlayerMoveInZone(player, 9);
		player:kickEvent(player:getDirector(), "noticeEvent", true);		
	else
		if (choice == 1) then	
			man0l0Quest = player:getQuest("Man0l0");
			player:runEventFunction("delegateEvent", player, man0l0Quest, "processEvent000_2", nil, nil, nil, nil);
		else
			player:endEvent();
		end
	end
	
end