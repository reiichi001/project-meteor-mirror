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

    local worldManager = GetWorldManager();
    
	yshtola = GetWorldManager():SpawnBattleNpcById(6, contentArea);
	stahlmann = GetWorldManager():SpawnBattleNpcById(7, contentArea);
	
	mob1 = GetWorldManager():SpawnBattleNpcById(3, contentArea);
	mob2 = GetWorldManager():SpawnBattleNpcById(4, contentArea);
	mob3 = GetWorldManager():SpawnBattleNpcById(5, contentArea);
	
    local added = false;
    for _, player in pairs(players) do
        if player.currentParty and not added then
            player.currentParty.AddMember(yshtola);
            player.currentParty.AddMember(stahlmann);
            added = true;
        end;
        -- dont let player die
        player.SetModifier(modifiersGlobal.MinimumHpLock, 1);
  
        contentGroup:AddMember(player);
    end;
	contentGroup:AddMember(director);
	contentGroup:AddMember(yshtola);
	contentGroup:AddMember(stahlmann);
	contentGroup:AddMember(mob1);
	contentGroup:AddMember(mob2);
	contentGroup:AddMember(mob3);
end

function onEventStarted(player, actor, triggerName)	

	man0g0Quest = player:GetQuest("Man0g0");
	startTutorialMode(player);
	callClientFunction(player, "delegateEvent", player, man0g0Quest, "processTtrBtl001", nil, nil, nil);
	player:EndEvent();
	waitForSignal("playerActive");
	wait(2); --If this isn't here, the scripts bugs out. TODO: Find a better alternative.
	kickEventContinue(player, actor, "noticeEvent", "noticeEvent");	
	callClientFunction(player, "delegateEvent", player, man0g0Quest, "processTtrBtl002", nil, nil, nil);
	player:EndEvent();
    
	closeTutorialWidget(player);
	wait(3);
    
    man0g0Quest:NextPhase(5);
	openTutorialWidget(player, CONTROLLER_KEYBOARD, TUTORIAL_TP);
    wait(5);
	
    man0g0Quest:NextPhase(6);
	closeTutorialWidget(player);
	
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