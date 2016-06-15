require("/quests/man/man0l0")

function init(npc)
	return "/Chara/Npc/Populace/PopulaceTutorial", false, false, false, false, false, npc:GetActorClassId(), false, false, 0, 1, "TEST";	
end

function onSpawn(player, npc)

	man0l0Quest = player:GetQuest("Man0l0");
	
	if (man0l0Quest ~= nil) then
		if (man0l0Quest ~= nil and man0l0Quest:GetQuestFlag(MAN0L0_FLAG_MINITUT_DONE1) == true and man0l0Quest:GetQuestFlag(MAN0L0_FLAG_MINITUT_DONE2) == true and man0l0Quest:GetQuestFlag(MAN0L0_FLAG_MINITUT_DONE3) == true) then
			player:SetEventStatus(npc, "pushDefault", true, 0x2);
			npc:SetQuestGraphic(player, 0x3);
		else
			player:SetEventStatus(npc, "pushDefault", true, 0x2);
			npc:SetQuestGraphic(player, 0x3);
		end
	end
	
end

function onEventStarted(player, npc, triggerName)

	if (triggerName == "pushDefault") then
		man0l0Quest = GetStaticActor("Man0l0");
		player:RunEventFunction("delegateEvent", player, man0l0Quest, "processEventNewRectAsk", nil);
	else
		player:EndEvent();
	end
	
end

function onEventUpdate(player, npc, resultId, choice)

	if (resultId == 0x2B9EBC42) then
		player:EndEvent();
		player:SetDirector("QuestDirectorMan0l001", true);
		
		worldMaster = GetWorldMaster();
		player:SendGameMessage(player, worldMaster, 34108, 0x20);	
		player:SendGameMessage(player, worldMaster, 50011, 0x20);	

		GetWorldManager():DoPlayerMoveInZone(player, 9);
		player:KickEvent(player:GetDirector(), "noticeEvent", true);		
	else
		if (choice == 1) then	
			man0l0Quest = player:GetQuest("Man0l0");
			player:RunEventFunction("delegateEvent", player, man0l0Quest, "processEvent000_2", nil, nil, nil, nil);
		else
			player:EndEvent();
		end
	end
	
end