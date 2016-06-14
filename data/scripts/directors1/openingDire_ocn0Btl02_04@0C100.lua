require("/quests/man/man0l0")

function onEventStarted(player, actor, triggerName)	
	
	man0l0Quest = getStaticActor("Man0l0");	
	player:runEventFunction("delegateEvent", player, man0l0Quest, "processTtrNomal001withHQ", nil, nil, nil, nil);
	--player:runEventFunction("delegateEvent", player, man0l0Quest, "processEvent000_1", nil, nil, nil, nil);
	
end

function onEventUpdate(player, npc, resultId)
	
	player:endEvent();
	
end

function onTalked(player, npc)
	
	man0l0Quest = player:getQuest("Man0l0");
	
	if (man0l0Quest ~= nil) then
		if (man0l0Quest ~= nil and man0l0Quest:GetQuestFlag(MAN0L0_FLAG_MINITUT_DONE1) == true and man0l0Quest:GetQuestFlag(MAN0L0_FLAG_MINITUT_DONE2) == true and man0l0Quest:GetQuestFlag(MAN0L0_FLAG_MINITUT_DONE3) == true) then
		
			doorNpc = getWorldManager():GetActorInWorld(1090025);		
			player:setEventStatus(doorNpc, "pushDefault", true, 0x2);
			doorNpc:setQuestGraphic(player, 0x3);		
			
		end
	end
	
end