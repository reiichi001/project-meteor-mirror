require ("global")
require ("quests/man/man0l0")
require ("quests/man/man0g0")
require ("quests/man/man0u0")

function init()
	return "/Director/OpeningDirector";
end

function onEventStarted(player, actor, triggerName)		
	
	if (player:HasQuest(110001) == true) then
		quest = player:GetQuest("Man0l0");
		callClientFunction(player, "delegateEvent", player, quest, "processTtrNomal001withHQ", nil, nil, nil);		
	elseif (player:HasQuest(110005) == true) then
		quest = player:GetQuest("Man0g0");
		callClientFunction(player, "delegateEvent", player, quest, "processTtrNomal001withHQ", nil, nil, nil);
	elseif (player:HasQuest(110009) == true) then
		quest = player:GetQuest("Man0u0");
		callClientFunction(player, "delegateEvent", player, quest, "processTtrNomal001withHQ", nil, nil, nil);
	end
	
	player:EndEvent();
	
end

function onUpdate()
end

function onTalkEvent(player, npc)

	if (player:HasQuest(110001) == true) then
		man0l0Quest = player:GetQuest("man0l0");
		
		if (man0l0Quest:GetQuestFlag(MAN0L0_FLAG_MINITUT_DONE1) == true and man0l0Quest:GetQuestFlag(MAN0L0_FLAG_MINITUT_DONE2) == true and man0l0Quest:GetQuestFlag(MAN0L0_FLAG_MINITUT_DONE3) == true) then
			doorNpc = GetWorldManager():GetActorInWorldByUniqueId("exit_door");		
			player:SetEventStatus(doorNpc, "pushDefault", true, 0x2);
			doorNpc:SetQuestGraphic(player, 0x3);
		end
	elseif (player:HasQuest(110005) == true) then	
		man0g0Quest = player:GetQuest("man0g0");
		if (man0g0Quest:GetQuestFlag(MAN0L0_FLAG_STARTED_TALK_TUT) == true and man0g0Quest:GetQuestFlag(MAN0G0_FLAG_MINITUT_DONE1) == false) then
			papalymo = GetWorldManager():GetActorInWorldByUniqueId("papalymo");	
			papalymo:SetQuestGraphic(player, 0x2);
		elseif (man0g0Quest:GetQuestFlag(MAN0L0_FLAG_STARTED_TALK_TUT) == true and man0g0Quest:GetQuestFlag(MAN0G0_FLAG_MINITUT_DONE1) == true) then			
			yda = GetWorldManager():GetActorInWorldByUniqueId("yda");	
			yda:SetQuestGraphic(player, 0x2);
		end
	elseif (player:HasQuest(110009) == true) then	
		man0u0Quest = player:GetQuest("man0u0");
		if (man0u0Quest:GetQuestFlag(MAN0U0_FLAG_MINITUT_DONE1) == true and man0u0Quest:GetQuestFlag(MAN0U0_FLAG_MINITUT_DONE2) == true and man0u0Quest:GetQuestFlag(MAN0U0_FLAG_MINITUT_DONE3) == true) then			
			exitTriggerNpc = GetWorldManager():GetActorInWorldByUniqueId("exit_trigger");		
			player:SetEventStatus(exitTriggerNpc, "pushDefault", true, 0x2);
			exitTriggerNpc:SetQuestGraphic(player, 0x2);					
		end
	end

end

function onPushEvent(player, npc)
end

function onCommandEvent(player, command)
end

function onEventUpdate(player, npc)
end

function onCommand(player, command)	
end