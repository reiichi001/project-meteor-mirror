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
	player:SetMod(modifiersGlobal.MinimumHpLock, 1);
	player:SendMessage(0x20, "", "Starting");
	startTutorialMode(player);
	callClientFunction(player, "delegateEvent", player, man0g0Quest, "processTtrBtl001", nil, nil, nil);
	player:EndEvent();
	player:SendMessage(0x20, "", "Waiting for player active");
	waitForSignal("playerActive");
	player:SendMessage(0x20, "", "player active");
	wait(1); --If this isn't here, the scripts bugs out. TODO: Find a better alternative.
	kickEventContinue(player, actor, "noticeEvent", "noticeEvent");	
	callClientFunction(player, "delegateEvent", player, man0g0Quest, "processTtrBtl002", nil, nil, nil);
	player:SendMessage(0x20, "", "processTtrBtl002 called");
	player:EndEvent();

	--Combat portion of tutorial
	
	if player:IsDiscipleOfWar() then
		player:SendMessage(0x20, "", "Is DoW");
		waitForSignal("playerAttack");
		closeTutorialWidget(player);
		showTutorialSuccessWidget(player, 9055); --Open TutorialSuccessWidget for attacking enemy
		openTutorialWidget(player, CONTROLLER_KEYBOARD, TUTORIAL_TP);
		waitForSignal("tpOver1000");
		player:SetMod(modifiersGlobal.MinimumTpLock, 1000);
		closeTutorialWidget(player);
		openTutorialWidget(player, CONTROLLER_KEYBOARD, TUTORIAL_WEAPONSKILLS);
		waitForSignal("weaponskillUsed");
		player:SetMod(modifiersGlobal.MinimumTpLock, 0);
		closeTutorialWidget(player);
		showTutorialSuccessWidget(player, 9065); --Open TutorialSuccessWidget for weapon skill
	elseif player:IsDiscipleOfMagic() then
		player:SendMessage(0x20, "", "Is DoM");
		openTutorialWidget(player, CONTROLLER_KEYBOARD, TUTORIAL_CASTING);
		waitForSignal("spellUsed");
		closeTutorialWidget(player);
	elseif player:IsDiscipleOfHand() then
		waitForSignal("abilityUsed");
	elseif player:IsDiscipleOfLand() then
		waitForSignal("abilityUsed");
	end
	
	player:SendMessage(0x20, "", "Waiting for mobkill1");
	waitForSignal("mobkill"); --Should be wait for mobkill
	player:SendMessage(0x20, "", "Waiting for mobkill2");
	waitForSignal("mobkill");
	player:SendMessage(0x20, "", "Waiting for mobkill3");
	waitForSignal("mobkill");
	worldMaster = GetWorldMaster();
	player:SetMod(modifiersGlobal.MinimumHpLock, 0);
	player:SendMessage(0x20, "", "Sending data packet 'attention'");
	player:SendDataPacket("attention", worldMaster, "", 51073, 2);
	wait(5);
	player:SendMessage(0x20, "", "Disengaging");
	player:Disengage(0x0000);
	wait(5);
	player:SendMessage(0x20, "", "NextPhase(10)");
	man0g0Quest:NextPhase(10);	
	wait(5);
	player:SendMessage(0x20, "", "ProcessEvent020_1");
	callClientFunction(player, "delegateEvent", player, man0g0Quest, "processEvent020_1", nil, nil, nil);

	wait(5);
	
	player:SendMessage(0x20, "", "Changing music");
	player:ChangeMusic(7);
	wait(5);
	
	player:SendMessage(0x20, "", "Kick notice event");
	kickEventContinue(player, actor, "noticeEvent", "noticeEvent");
	wait(5);
	
	player:SendMessage(0x20, "", "ContentFinished");
	player:GetZone():ContentFinished();	
	wait(5);
	player:SendMessage(0x20, "", "Remove from party");
	player:RemoveFromCurrentPartyAndCleanup();
    --player:EndEvent();
    --GetWorldManager():DoZoneChange(player, 155, "PrivateAreaMasterPast", 1, 15, 175.38, -1.21, -1156.51, -2.1);
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
	
	player:EndEvent();	
	
	wait(5);
	player:SendMessage(0x20, "", "Zone change");
	GetWorldManager():DoZoneChange(player, 155, "PrivateAreaMasterPast", 1, 15, 175.38, -1.21, -1156.51, -2.1);
	
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