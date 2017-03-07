require ("global")
require ("quests/man/man0u0")

function onSpawn(player, npc)
	npc:SetQuestGraphic(player, 0x2);	
end

function onEventStarted(player, npc, triggerName)
	man0u0Quest = player:GetQuest("Man0u0");
	
	if (man0u0Quest ~= nil) then
		if (man0u0Quest:GetQuestFlag(MAN0U0_FLAG_MINITUT_DONE1) == false) then
			npc:SetQuestGraphic(player, 0x2);
		end
		
		if (man0u0Quest:GetQuestFlag(MAN0U0_FLAG_TUTORIAL3_DONE) == true) then
			player:SetEventStatus(npc, "pushDefault", false, 0x2);		
		end
	end
	
	if (man0u0Quest ~= nil) then
		if (triggerName == "pushDefault") then		
				callClientFunction(player, "delegateEvent", player, man0u0Quest, "processTtrNomal002", nil, nil, nil);			
		elseif (triggerName == "talkDefault") then		
			--Is doing talk tutorial?
			if (man0u0Quest:GetQuestFlag(MAN0U0_FLAG_TUTORIAL3_DONE) == false) then
				player:SetEventStatus(npc, "pushDefault", false, 0x2);
				callClientFunction(player, "delegateEvent", player, man0u0Quest, "processTtrNomal003", nil, nil, nil);
				man0u0Quest:SetQuestFlag(MAN0U0_FLAG_TUTORIAL3_DONE, true);				
				npc:SetQuestGraphic(player, 0x2);
				man0u0Quest:SaveData();
				
				player:GetDirector("OpeningDirector"):onTalkEvent(player, npc);
			--Was he talked to for the mini tutorial?
			else				
				callClientFunction(player, "delegateEvent", player, man0u0Quest, "processTtrMini001", nil, nil, nil);
				if (man0u0Quest:GetQuestFlag(MAN0U0_FLAG_MINITUT_DONE1) == false) then
					npc:SetQuestGraphic(player, 0x0);
					man0u0Quest:SetQuestFlag(MAN0U0_FLAG_MINITUT_DONE1, true);
					man0u0Quest:SaveData();										
				end

			end

			player:GetDirector("OpeningDirector"):onTalkEvent(player, npc);
		else
			player:EndEvent();
		end
	end
	
	player:EndEvent();
end