require("/quests/man/man0g0")

function onEventStarted(player, actor, triggerName)	

	man0g0Quest = GetStaticActor("Man0g0");	
	player:RunEventFunction("delegateEvent", player, man0g0Quest, "processTtrNomal001withHQ", nil, nil, nil, nil);
	
end

function onEventUpdate(player, npc, resultId)	

	player:EndEvent();
	
end

function onTalked(player, npc)
	
	man0g0Quest = player:GetQuest("Man0g0");
	
	if (man0g0Quest ~= nil) then
	
		yda = GetWorldManager():GetActorInWorld(1000009);
		papalymo = GetWorldManager():GetActorInWorld(1000010);			
	
		if (man0g0Quest:GetQuestFlag(MAN0G0_FLAG_TUTORIAL1_DONE) == false) then		
			yda:SetQuestGraphic(player, 0x0);
			papalymo:SetQuestGraphic(player, 0x2);
		else
			if (man0g0Quest:GetQuestFlag(MAN0G0_FLAG_MINITUT_DONE1) == true) then
				yda:SetQuestGraphic(player, 0x2);
				papalymo:SetQuestGraphic(player, 0x0);
			end			
		end
		
	end
	
end