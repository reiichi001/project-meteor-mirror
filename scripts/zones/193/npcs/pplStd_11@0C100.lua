require("/quests/man/man0l0")

function init(npc)
	return "/Chara/Npc/Populace/PopulaceTutorial", false, false, false, false, false, npc.getActorClassId(), false, false, 0, 1, "TEST";	
end

function onSpawn(player, npc)
	
	man0l0Quest = player:getQuest("Man0l0");	
	
	if (man0l0Quest ~= nil) then
		if (man0l0Quest:GetQuestFlag(MAN0L0_FLAG_MINITUT_DONE1) == false) then
			npc:setQuestGraphic(player, 0x2);
		end
		
		if (man0l0Quest:GetQuestFlag(MAN0L0_FLAG_TUTORIAL3_DONE) == true) then
			player:setEventStatus(npc, "pushDefault", false, 0x2);		
		end
	end
	
end

function onEventStarted(player, npc, triggerName)

	man0l0Quest = player:getQuest("Man0l0");
	
	if (man0l0Quest ~= nil) then	
	
		if (triggerName == "pushDefault") then
			player:runEventFunction("delegateEvent", player, man0l0Quest, "processTtrNomal002", nil, nil, nil);			
		elseif (triggerName == "talkDefault") then		
			--Is doing talk tutorial?
			if (man0l0Quest:GetQuestFlag(MAN0L0_FLAG_TUTORIAL3_DONE) == false) then
				player:setEventStatus(npc, "pushDefault", false, 0x2);
				player:runEventFunction("delegateEvent", player, man0l0Quest, "processTtrNomal003", nil, nil, nil);
				man0l0Quest:SetQuestFlag(MAN0L0_FLAG_TUTORIAL3_DONE, true);				
				npc:setQuestGraphic(player, 0x2);
				man0l0Quest:SaveData();
				
				player:getDirector():onTalked(npc);
			--Was he talked to for the mini tutorial?
			else				
				player:runEventFunction("delegateEvent", player, man0l0Quest, "processTtrMini001", nil, nil, nil);
				
				if (man0l0Quest:GetQuestFlag(MAN0L0_FLAG_MINITUT_DONE1) == false) then
					npc:setQuestGraphic(player, 0x0);
					man0l0Quest:SetQuestFlag(MAN0L0_FLAG_MINITUT_DONE1, true);
					man0l0Quest:SaveData();
					
					player:getDirector():onTalked(npc);
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
	player:endEvent();
end