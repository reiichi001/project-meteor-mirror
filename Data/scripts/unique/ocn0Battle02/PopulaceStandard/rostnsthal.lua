require ("global")
require ("quests/man/man0l0")

function onSpawn(player, npc)
	man0l0Quest = player:GetQuest("Man0l0");	
	
	if (man0l0Quest ~= nil) then
		if (man0l0Quest:GetQuestFlag(MAN0L0_FLAG_MINITUT_DONE1) == false) then
			npc:SetQuestGraphic(player, 0x2);
		end
		
		if (man0l0Quest:GetQuestFlag(MAN0L0_FLAG_STARTED_TALK_TUT) == true) then
			player:SetEventStatus(npc, "pushDefault", false, 0x2);		
		end
	end
end

function onEventStarted(player, npc, triggerName)
	man0l0Quest = player:GetQuest("Man0l0");
	
	if (man0l0Quest ~= nil) then	
	
		if (triggerName == "pushDefault") then
			callClientFunction(player, "delegateEvent", player, man0l0Quest, "processTtrNomal002", nil, nil, nil);			
		elseif (triggerName == "talkDefault") then		
			--Is doing talk tutorial?
			if (man0l0Quest:GetQuestFlag(MAN0L0_FLAG_STARTED_TALK_TUT) == false) then
				player:SetEventStatus(npc, "pushDefault", false, 0x2);
				callClientFunction(player, "delegateEvent", player, man0l0Quest, "processTtrNomal003", nil, nil, nil);
				man0l0Quest:SetQuestFlag(MAN0L0_FLAG_STARTED_TALK_TUT, true);				
				npc:SetQuestGraphic(player, 0x2);
				man0l0Quest:SaveData();
				
				player:GetDirector("OpeningDirector"):onTalkEvent(player, npc);
			--Was he talked to for the mini tutorial?
			else				
				callClientFunction(player, "delegateEvent", player, man0l0Quest, "processTtrMini001", nil, nil, nil);
				
				if (man0l0Quest:GetQuestFlag(MAN0L0_FLAG_MINITUT_DONE1) == false) then
					npc:SetQuestGraphic(player, 0x0);
					man0l0Quest:SetQuestFlag(MAN0L0_FLAG_MINITUT_DONE1, true);
					man0l0Quest:SaveData();
					
					player:GetDirector("OpeningDirector"):onTalkEvent(player, npc);
				end
			end					
		end	
	end	
	
	player:EndEvent();
end