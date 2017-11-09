require ("global")
require ("tutorial")
require ("modifiers")
require ("quests/man/man0g0")

--processTtrBtl001: Active Mode Tutorial
--processTtrBtl002: Targetting Tutorial (After active mode done)

function init()
	return "/Director/Quest/QuestDirectorMan0g001";
end

function onCreateContentArea(players, director, contentArea, contentGroup)
	director:StartContentGroup();
end

function onEventStarted(player, actor, triggerName)
	man0g0Quest = player:GetQuest("Man0g0");
	startTutorialMode(player);
	callClientFunction(player, "delegateEvent", player, man0g0Quest, "processTtrBtl001", nil, nil, nil);
	player:EndEvent();
	waitForSignal("playerActive");
	wait(1); --If this isn't here, the scripts bugs out. TODO: Find a better alternative.
	kickEventContinue(player, actor, "noticeEvent", "noticeEvent");	
	callClientFunction(player, "delegateEvent", player, man0g0Quest, "processTtrBtl002", nil, nil, nil);
	player:EndEvent();
	waitForSignal("playerAttack");
	closeTutorialWidget(player);
	showTutorialSuccessWidget(player, 9055); --Open TutorialSuccessWidget for attacking enemy
	wait(3);
	openTutorialWidget(player, CONTROLLER_KEYBOARD, TUTORIAL_TP);
	waitForSignal("tpOver1000");
	closeTutorialWidget(player);
	openTutorialWidget(player, CONTROLLER_KEYBOARD, TUTORIAL_WEAPONSKILLS);

	if player:IsDiscipleOfWar() then
		waitForSignal("weaponskillUsed"); --Should be wait for weaponskillUsed signal
	elseif player:IsDiscipleOfMagic() then
		waitForSignal("spellUsed")
	elseif player:IsDiscipleOfHand() then
		waitForSignal("abilityUsed")
	elseif player:IsDiscipleOfLand() then
		waitForSignal("abilityUsed")
	end
	closeTutorialWidget(player);
	showTutorialSuccessWidget(player, 9065); --Open TutorialSuccessWidget for weapon skill
	
	waitForSignal("mobkill"); --Should be wait for mobkill
	waitForSignal("mobkill");
	waitForSignal("mobkill");
	worldMaster = GetWorldMaster();
	player:SendDataPacket("attention", worldMaster, "", 51073, 2);
	wait(7);
	player:ChangeMusic(7);
	player:ChangeState(0); 
	kickEventContinue(player, actor, "noticeEvent", "noticeEvent");
	callClientFunction(player, "delegateEvent", player, man0g0Quest, "processEvent020_1", nil, nil, nil);
	
	player:GetZone():ContentFinished();
    player:EndEvent();
    GetWorldManager():DoZoneChange(player, 155, "PrivateAreaMasterPast", 1, 15, 175.38, -1.21, -1156.51, -2.1);
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
	
	--man0g0Quest:NextPhase(10);	
	--player:EndEvent();	
	
	--GetWorldManager():DoZoneChange(player, 155, "PrivateAreaMasterPast", 1, 15, 175.38, -1.21, -1156.51, -2.1);
	
end

function onUpdate(deltaTime, area)
end

function onTalkEvent(player, npc)

end

function onPushEvent(player, npc)
end

function onCommandEvent(player, command)

end

function onEventUpdate(player, npc)
end

function onCommand(player, command)	
end

function main(director, contentGroup)
    onCreateContentArea(director:GetPlayerMembers(), director, director:GetZone(), contentGroup);
end;