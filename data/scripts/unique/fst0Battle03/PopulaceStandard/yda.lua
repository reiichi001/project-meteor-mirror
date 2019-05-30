require ("global")
require ("quests/man/man0g0")

function onSpawn(player, npc)
	npc:SetQuestGraphic(player, 0x2);	
end

function onEventStarted(player, npc, triggerName)
	man0g0Quest = player:GetQuest("Man0g0");
	print("Got Quest Man0g0");
	if (man0g0Quest ~= nil) then	
		
		print("Man0g0Quest is not nil");
		if (triggerName == "pushDefault") then
			callClientFunction(player, "delegateEvent", player, man0g0Quest, "processTtrNomal002", nil, nil, nil);			
		elseif (triggerName == "talkDefault") then		
			--Is doing talk tutorial?
			if (man0g0Quest:GetQuestFlag(MAN0L0_FLAG_STARTED_TALK_TUT) == false) then
				callClientFunction(player, "delegateEvent", player, man0g0Quest, "processTtrNomal003", nil, nil, nil);			
				player:SetEventStatus(npc, "pushDefault", false, 0x2);
				npc:SetQuestGraphic(player, 0x0);
				man0g0Quest:SetQuestFlag(MAN0L0_FLAG_STARTED_TALK_TUT, true);
				man0g0Quest:SaveData();
				player:GetDirector("OpeningDirector"):onTalkEvent(player, npc);
			--Was she talked to after papalymo?
			else
				print("Making content area");
				if (man0g0Quest:GetQuestFlag(MAN0G0_FLAG_MINITUT_DONE1) == true) then				

					player:EndEvent();
		
					contentArea = player:GetZone():CreateContentArea(player, "/Area/PrivateArea/Content/PrivateAreaMasterSimpleContent", "man0g01", "SimpleContent30010", "Quest/QuestDirectorMan0g001");
		
					if (contentArea == nil) then
						player:EndEvent();
						return;
					end
		
					director = contentArea:GetContentDirector();		
					--player:AddDirector(director);		
					director:StartDirector(false);
					
					player:KickEvent(director, "noticeEvent", true);
					player:SetLoginDirector(director);		
					
					print("Content area and director made");
					player:ChangeState(0);
					GetWorldManager():DoZoneChangeContent(player, contentArea, 362.4087, 4, -703.8168, 1.5419, 16);
					print("Zone Change");
					return;
				else
					callClientFunction(player, "delegateEvent", player, man0g0Quest, "processEvent000_1", nil, nil, nil);
				end
			end
		end		
	end	
	
	player:EndEvent();
end