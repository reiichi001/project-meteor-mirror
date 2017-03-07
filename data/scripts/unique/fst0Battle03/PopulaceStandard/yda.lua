require ("global")
require ("quests/man/man0g0")

function onSpawn(player, npc)
	npc:SetQuestGraphic(player, 0x2);	
end

function onEventStarted(player, npc, triggerName)
	man0g0Quest = player:GetQuest("Man0g0");
	
	if (man0g0Quest ~= nil) then	
	
		if (triggerName == "pushDefault") then
			callClientFunction(player, "delegateEvent", player, man0g0Quest, "processTtrNomal002", nil, nil, nil);			
		elseif (triggerName == "talkDefault") then		
			--Is doing talk tutorial?
			if (man0g0Quest:GetQuestFlag(MAN0G0_FLAG_TUTORIAL1_DONE) == false) then
				callClientFunction(player, "delegateEvent", player, man0g0Quest, "processTtrNomal003", nil, nil, nil);			
				player:SetEventStatus(npc, "pushDefault", false, 0x2);
				npc:SetQuestGraphic(player, 0x0);
				player:GetDirector("OpeningDirector"):onTalkEvent(player, npc);
				man0g0Quest:SetQuestFlag(MAN0G0_FLAG_TUTORIAL1_DONE, true);				
				man0g0Quest:SaveData();
			--Was she talked to after papalymo?
			else
				if (man0g0Quest:GetQuestFlag(MAN0G0_FLAG_MINITUT_DONE1) == true) then				

					player:EndEvent();
		
					worldMaster = GetWorldMaster();
					player:SendGameMessage(player, worldMaster, 34108, 0x20);	
					player:SendGameMessage(player, worldMaster, 50011, 0x20);	

					director = player:GetZone():CreateDirector("Quest/QuestDirectorMan0g001");
					player:KickEvent(director, "noticeEvent", true);
					player:AddDirector(director);
					player:SetLoginDirector(director);
					
					GetWorldManager():DoZoneChange(player, 10);				
					return;
				else
					callClientFunction(player, "delegateEvent", player, man0g0Quest, "processEvent000_1", nil, nil, nil);
				end
			end
		end		
	end	
	
	player:EndEvent();
end