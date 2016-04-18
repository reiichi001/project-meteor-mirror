require("/quests/man/man0g0")

function init(npc)
	return "/Chara/Npc/Populace/PopulaceStandard", false, false, false, false, false, npc.getActorClassId(), false, false, 0, 1, "TEST";	
end

function onSpawn(player, npc)
	npc:setQuestGraphic(player, 0x2);	
end

function onEventStarted(player, npc, triggerName)

	man0g0Quest = player:getQuest("Man0g0");
	
	if (man0g0Quest ~= nil) then	
	
		if (triggerName == "pushDefault") then
			player:runEventFunction("delegateEvent", player, man0g0Quest, "processTtrNomal002", nil, nil, nil);			
		elseif (triggerName == "talkDefault") then		
			if (man0g0Quest:GetQuestFlag(MAN0G0_FLAG_TUTORIAL1_DONE) == false) then
				player:runEventFunction("delegateEvent", player, man0g0Quest, "processTtrNomal003", nil, nil, nil);			
				player:setEventStatus(npc, "pushDefault", false, 0x2);
				player:getDirector():onTalked(npc);			
				man0g0Quest:SetQuestFlag(MAN0G0_FLAG_TUTORIAL1_DONE, true);				
				man0g0Quest:SaveData();
			else
				if (man0g0Quest:GetQuestFlag(MAN0G0_FLAG_MINITUT_DONE1) == true) then
					man0g0Quest:SetQuestFlag(MAN0G0_FLAG_TUTORIAL2_DONE, true);
					player:runEventFunction("delegateEvent", player, man0g0Quest, "processEvent010_1", nil, nil, nil);					
				else
					player:runEventFunction("delegateEvent", player, man0g0Quest, "processEvent000_1", nil, nil, nil);
				end
			end
		else
			player:endEvent();
		end
	else
		player:endEvent(); --Should not be here w.o this quest
	end	
	
end

function onEventUpdate(player, npc)	

	man0g0Quest = player:getQuest("Man0g0");

	if (man0g0Quest:GetQuestFlag(MAN0G0_FLAG_TUTORIAL2_DONE) == true) then
		player:endEvent();
		player:setDirector("QuestDirectorMan0g001", true);

		worldMaster = getWorldMaster();
		player:sendGameMessage(player, worldMaster, 34108, 0x20);	
		player:sendGameMessage(player, worldMaster, 50011, 0x20);	

		getWorldManager():DoPlayerMoveInZone(player, 10);
		player:kickEvent(player:getDirector(), "noticeEvent", true);
	else
		player:endEvent();
	end
	
end