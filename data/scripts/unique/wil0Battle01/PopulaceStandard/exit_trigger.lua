require ("global")
require ("quests/man/man0u0")

function onSpawn(player, npc)

	man0u0Quest = player:GetQuest("Man0u0");
	
	if (man0u0Quest ~= nil) then
		player:SetEventStatus(npc, "pushDefault", true, 0x2);
		if (man0u0Quest ~= nil and man0u0Quest:GetQuestFlag(MAN0U0_FLAG_MINITUT_DONE1) == true and man0U0Quest:GetQuestFlag(MAN0U0_FLAG_MINITUT_DONE2) == true and man0u0Quest:GetQuestFlag(MAN0U0_FLAG_MINITUT_DONE3) == true) then			
			npc:SetQuestGraphic(player, 0x3);
		else			
			npc:SetQuestGraphic(player, 0x0);
		end
	end
	
end

function onEventStarted(player, npc, triggerName)		
	man0u0Quest = GetStaticActor("Man0u0");	
	
	if (man0u0Quest:GetQuestFlag(MAN0U0_FLAG_MINITUT_DONE1) ~= true) then
		print "AAAA"
	end
	
	if (man0u0Quest ~= nil and man0u0Quest:GetQuestFlag(MAN0U0_FLAG_MINITUT_DONE1) == true and man0u0Quest:GetQuestFlag(MAN0U0_FLAG_MINITUT_DONE2) == true and man0u0Quest:GetQuestFlag(MAN0U0_FLAG_MINITUT_DONE3) == true) then		
		player:EndEvent();
		
		contentArea = player:GetZone():CreateContentArea(player, "/Area/PrivateArea/Content/PrivateAreaMasterSimpleContent", "man0u01", "SimpleContent30079", "Quest/QuestDirectorMan0u001");
		
		if (contentArea == nil) then
			player:EndEvent();
			return;
		end
		
		director = contentArea:GetContentDirector();		
		player:AddDirector(director);		
		director:StartDirector(false);
		
		player:KickEvent(director, "noticeEvent", true);
		player:SetLoginDirector(director);		
					
		GetWorldManager():DoZoneChangeContent(player, contentArea, -24.34, 192, 34.22, 0.78, 16);
	end
	
end