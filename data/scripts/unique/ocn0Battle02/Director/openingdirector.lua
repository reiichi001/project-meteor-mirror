require ("global")
require("/quests/man/man0l0")

function onEventStarted(player, actor, triggerName)	

	man0l0Quest = GetStaticActor("Man0l0");	
	callClientFunction(player, "delegateEvent", player, man0l0Quest, "processTtrNomal001withHQ", nil, nil, nil, nil);
	player:EndEvent();
	
end

function onTalked(player, npc)
	
	man0l0Quest = player:GetQuest("Man0l0");
	
	if (man0l0Quest ~= nil) then
		if (man0l0Quest ~= nil and man0l0Quest:GetQuestFlag(MAN0L0_FLAG_MINITUT_DONE1) == true and man0l0Quest:GetQuestFlag(MAN0L0_FLAG_MINITUT_DONE2) == true and man0l0Quest:GetQuestFlag(MAN0L0_FLAG_MINITUT_DONE3) == true) then
		
			doorNpc = GetWorldManager():GetActorInWorld(1090025);		
			player:SetEventStatus(doorNpc, "pushDefault", true, 0x2);
			doorNpc:SetQuestGraphic(player, 0x3);		
			
		end
	end
	
end