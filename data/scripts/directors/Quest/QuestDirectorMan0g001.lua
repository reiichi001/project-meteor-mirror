require ("global")
require ("tutorial")
require ("modifiers")
require ("quests/man/man0g0")

--processTtrBtl001: Active Mode Tutorial
--processTtrBtl002: Targetting Tutorial (After active mode done)

function init()
	return "/Director/Quest/QuestDirectorMan0g001";
end

--Should we be using this to spawn mobs and drop Simplecontent?
function onCreateContentArea(players, director, contentArea, contentGroup)
	director:StartContentGroup();
end

function onEventStarted(player, actor, triggerName)
	man0g0Quest = player:GetQuest("Man0g0");
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
		waitForSignal("playerAttack");
		closeTutorialWidget(player);
		showTutorialSuccessWidget(player, 9055); --Open TutorialSuccessWidget for attacking enemy
		openTutorialWidget(player, CONTROLLER_KEYBOARD, TUTORIAL_TP);
		waitForSignal("tpOver1000");
		closeTutorialWidget(player);
		openTutorialWidget(player, CONTROLLER_KEYBOARD, TUTORIAL_WEAPONSKILLS);
		waitForSignal("weaponskillUsed");
		closeTutorialWidget(player);
		showTutorialSuccessWidget(player, 9065); --Open TutorialSuccessWidget for weapon skill
	elseif player:IsDiscipleOfMagic() then
		openTutorialWidget(player, CONTROLLER_KEYBOARD, TUTORIAL_CASTING);
		waitForSignal("spellUsed");
		closeTutorialWidget(player);
	elseif player:IsDiscipleOfHand() then
		waitForSignal("abilityUsed");
	elseif player:IsDiscipleOfLand() then
		waitForSignal("abilityUsed");
	end
	
	--Currently this requires the player to pass all the other signals first, need a way for signals to work out of order
	waitForSignal("mobkill");
	waitForSignal("mobkill");
	waitForSignal("mobkill");
	worldMaster = GetWorldMaster();
	wait(5);--Debug waits, get rid of these later
	player:Disengage(0x0000);
	wait(5);
	man0g0Quest:NextPhase(10);
	wait(5);
	--This doesn't work
	player:RemoveFromCurrentPartyAndCleanup();
	player:SendDataPacket("attention", worldMaster, "", 51073, 2);
	wait(5);
	player:ChangeMusic(7);
	wait(5);
	kickEventContinue(player, actor, "noticeEvent", "noticeEvent");
	wait(5);
	callClientFunction(player, "delegateEvent", player, man0g0Quest, "processEvent020_1", nil, nil, nil);
	wait(5);
	player:GetZone():ContentFinished();	
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