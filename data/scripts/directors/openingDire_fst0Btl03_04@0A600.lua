require("/quests/man/man0g0")

function onEventStarted(player, actor, triggerName)	

	man0g0Quest = getStaticActor("Man0g0");	
	player:runEventFunction("delegateEvent", player, man0g0Quest, "processTtrNomal001withHQ", nil, nil, nil, nil);
	
end

function onEventUpdate(player, npc, resultId)	

	player:endEvent();
	
end

function onTalked(player, npc)
	
	man0g0Quest = player:getQuest("Man0g0");
	
	if (man0g0Quest ~= nil) then
	
		yda = getWorldManager():GetActorInWorld(1000009);
		papalymo = getWorldManager():GetActorInWorld(1000010);			
	
		if (man0g0Quest:GetQuestFlag(MAN0G0_FLAG_TUTORIAL1_DONE) == false) then		
			yda:setQuestGraphic(player, 0x0);
			papalymo:setQuestGraphic(player, 0x2);
		else
			if (man0g0Quest:GetQuestFlag(MAN0G0_FLAG_MINITUT_DONE1) == true) then
				yda:setQuestGraphic(player, 0x2);
				papalymo:setQuestGraphic(player, 0x0);
			end			
		end
		
	end
	
end