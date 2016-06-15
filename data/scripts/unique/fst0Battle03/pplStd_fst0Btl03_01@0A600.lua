require("/quests/man/man0g0")

function init(npc)
	return "/Chara/Npc/Populace/PopulaceStandard", false, false, false, false, false, npc.GetActorClassId(), false, false, 0, 1, "TEST";	
end

function onSpawn(player, npc)
	npc:SetQuestGraphic(player, 0x2);	
end

function onEventStarted(player, npc, triggerName)

	man0g0Quest = player:GetQuest("Man0g0");
	
	if (man0g0Quest ~= nil) then	
	
		if (triggerName == "pushDefault") then
			player:RunEventFunction("delegateEvent", player, man0g0Quest, "processTtrNomal002", nil, nil, nil);			
		elseif (triggerName == "talkDefault") then		
			if (man0g0Quest:GetQuestFlag(MAN0G0_FLAG_TUTORIAL1_DONE) == false) then
				player:RunEventFunction("delegateEvent", player, man0g0Quest, "processTtrNomal003", nil, nil, nil);			
				player:SetEventStatus(npc, "pushDefault", false, 0x2);
				player:GetDirector():OnTalked(npc);			
				man0g0Quest:SetQuestFlag(MAN0G0_FLAG_TUTORIAL1_DONE, true);				
				man0g0Quest:SaveData();
			else
				if (man0g0Quest:GetQuestFlag(MAN0G0_FLAG_MINITUT_DONE1) == true) then
					man0g0Quest:SetQuestFlag(MAN0G0_FLAG_TUTORIAL2_DONE, true);
					player:RunEventFunction("delegateEvent", player, man0g0Quest, "processEvent010_1", nil, nil, nil);					
				else
					player:RunEventFunction("delegateEvent", player, man0g0Quest, "processEvent000_1", nil, nil, nil);
				end
			end
		else
			player:EndEvent();
		end
	else
		player:EndEvent(); --Should not be here w.o this quest
	end	
	
end

function onEventUpdate(player, npc)	

	man0g0Quest = player:GetQuest("Man0g0");

	if (man0g0Quest:GetQuestFlag(MAN0G0_FLAG_TUTORIAL2_DONE) == true) then
		player:EndEvent();
		player:SetDirector("QuestDirectorMan0g001", true);

		worldMaster = GetWorldMaster();
		player:SendGameMessage(player, worldMaster, 34108, 0x20);	
		player:SendGameMessage(player, worldMaster, 50011, 0x20);	

		GetWorldManager():DoPlayerMoveInZone(player, 10);
		player:KickEvent(player:GetDirector(), "noticeEvent", true);
	else
		player:EndEvent();
	end
	
end