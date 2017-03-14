require ("global")
require ("quests/man/man0l0")

--processTtrBtl001: Active Mode Tutorial
--processTtrBtl002: Targetting Tutorial (After active mode done)
--processTtrBtl003: Auto Attack Done
--processTtrBtl004: Tutorial Complete

--[[
12: TP
13: WeaponSkills

]]--

function init()
	return "/Director/Quest/QuestDirectorMan0l001";
end

function onEventStarted(player, actor, triggerName)	

	man0l0Quest = player:GetQuest("Man0l0");

	callClientFunction(player, "delegateEvent", player, man0l0Quest, "processTtrBtl001", nil, nil, nil);
	player:EndEvent();
	waitForSignal("playerActive");
	kickEventContinue(player, actor, "noticeEvent", "noticeEvent");
	callClientFunction(player, "delegateEvent", player, man0l0Quest, "processTtrBtl002", nil, nil, nil);
	player:EndEvent();
	
	
	wait(4);
	player:SendRequestedInfo(5); --Close TutorialWidget
	player:SendRequestedInfo(2, nil, nil, 9055, 9055, 9055); --Open TutorialSuccessWidget for attacking enemy
	wait(4);
	player:SendRequestedInfo(4, nil, nil, nil, 12); --Open TP TutorialWidget
	wait(4); --Should be wait for TP signal
	player:SendRequestedInfo(5); --Close TutorialWidget
	player:SendRequestedInfo(4, nil, nil, nil, 13); --Open WS TutorialWidget
	wait(4); --Should be wait for weaponskillUsed signal
	player:SendRequestedInfo(5); --Close TutorialWidget
	player:SendRequestedInfo(2, nil, nil, 9065, 9065, 9065); --Open TutorialSuccessWidget for weapon skill
	wait(4); --Should be wait for mobkill
	callClientFunction(player, "delegateEvent", player, man0l0Quest, "processEvent000_2", nil, nil, nil);		
	player:ChangeMusic(7);
	callClientFunction(player, "delegateEvent", player, man0l0Quest, "processEvent000_3", nil, nil, nil);	
	
	--sendDataPacket: Success
	--sendDataPacket: CloseWidget
	--IF DoW:
		--sendDataPacket: OpenWidget (TP)
		--IF TP REACHED:
		--sendDataPacket: CloseWidget
		--sendDataPacket: OpenWidget (WS)
		--IF WS USED:
		--sendDataPacket: Success
		--sendDataPacket: CloseWidget
	--ELSE MAGIC:
		--sendDataPacket: OpenWidget (DEFEAT ENEMY)
			
	--IF DEAD
	--sendDataPacket: Attention
	
	man0l0Quest:NextPhase(10);	
	player:EndEvent();
	
	GetWorldManager():DoZoneChange(player, 230, "PrivateAreaMasterPast", 1, 15, -826.868469, 6, 193.745865, -0.008368492);
	
end

function onUpdate()
end

function onTalkEvent(player, npc)

end

function onPushEvent(player, npc)
end

function onCommandEvent(player, command)

	quest = GetStaticActor("Man0l0");
	callClientFunction(player, "delegateEvent", player, quest, "processTtrBtl002", nil, nil, nil);	

end

function onEventUpdate(player, npc)
end

function onCommand(player, command)	
end