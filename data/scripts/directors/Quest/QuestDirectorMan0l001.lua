require ("global")
require ("tutorial")
require ("quests/man/man0l0")

function init()
	return "/Director/Quest/QuestDirectorMan0l001";
end

function onCreateContentArea(players, director, contentArea, contentGroup)

	yshtola = contentArea:SpawnActor(2290001, "yshtola", -8, 16.35, 6, 0.5);
	stahlmann = contentArea:SpawnActor(2290002, "stahlmann", 0, 16.35, 22, 3);
	
	mob1 = contentArea:SpawnActor(2205403, "mob1", -3.02+3, 17.35, 14.24, -2.81);
	mob2 = contentArea:SpawnActor(2205403, "mob2", -3.02, 17.35, 14.24, -2.81);
	mob3 = contentArea:SpawnActor(2205403, "mob3", -3.02-3, 17.35, 14.24, -2.81);
	
	for _, player in pairs(players) do
        contentGroup:AddMember(player);
    end;
	contentGroup:AddMember(director);
	contentGroup:AddMember(yshtola);
	contentGroup:AddMember(stahlmann);
	contentGroup:AddMember(mob1);
	contentGroup:AddMember(mob2);
	contentGroup:AddMember(mob3);
	
end

function onEventStarted(player, director, triggerName)	

	man0l0Quest = player:GetQuest("Man0l0");
	startTutorialMode(player);
	callClientFunction(player, "delegateEvent", player, man0l0Quest, "processTtrBtl001", nil, nil, nil);
	player:EndEvent();
	waitForSignal("playerActive");
	wait(1); --If this isn't here, the scripts bugs out. TODO: Find a better alternative.
	kickEventContinue(player, director, "noticeEvent", "noticeEvent");	
	callClientFunction(player, "delegateEvent", player, man0l0Quest, "processTtrBtl002", nil, nil, nil);
	player:EndEvent();
	wait(4);
	closeTutorialWidget(player);
	showTutorialSuccessWidget(player, 9055); --Open TutorialSuccessWidget for attacking enemy
	wait(3);
	openTutorialWidget(player, CONTROLLER_KEYBOARD, TUTORIAL_TP);
	wait(5);
	closeTutorialWidget(player);
	openTutorialWidget(player, CONTROLLER_KEYBOARD, TUTORIAL_WEAPONSKILLS);
	wait(4); --Should be wait for weaponskillUsed signal
	closeTutorialWidget(player);
	showTutorialSuccessWidget(player, 9065); --Open TutorialSuccessWidget for weapon skill
	
	wait(6); --Should be wait for mobkill
	worldMaster = GetWorldMaster();
	player:SendDataPacket("attention", worldMaster, "", 51073, 1);
	wait(7);
	player:ChangeMusic(7);
	player:ChangeState(0); 
	kickEventContinue(player, director, "noticeEvent", "noticeEvent");
	callClientFunction(player, "delegateEvent", player, man0l0Quest, "processEvent000_3", nil, nil, nil);	
	
	--[[
	IF DoW:
		OpenWidget (TP)
		IF TP REACHED:
			CloseWidget
			OpenWidget (WS)
		IF WS USED:
			Success
			CloseWidget
	ELSE MAGIC:
		OpenWidget (DEFEAT ENEMY)			
	]]
	
	man0l0Quest:NextPhase(10);	
	player:EndEvent();
	
	player:GetZone():ContentFinished();
	GetWorldManager():DoZoneChange(player, 230, "PrivateAreaMasterPast", 1, 15, -826.868469, 6, 193.745865, -0.008368492);
	
end
