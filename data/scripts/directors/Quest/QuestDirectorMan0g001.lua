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
    
	yshtola = GetWorldManager().SpawnBattleNpcById(6, contentArea);
	stahlmann = GetWorldManager().SpawnBattleNpcById(7, contentArea);
	
	mob1 = GetWorldManager().SpawnBattleNpcById(3, contentArea);
	mob2 = GetWorldManager().SpawnBattleNpcById(4, contentArea);
	mob3 = GetWorldManager().SpawnBattleNpcById(5, contentArea);
	
    local added = false;
    for i = 0, players.Count do
        local player = players[i];
		print("asses "..players.Count)
        if player.currentParty and not added then
			print("shitness")
            player.currentParty.members:Add(yshtola.actorId);
            print("cunt")
            player.currentParty.members:Add(stahlmann.actorId);
            print("dickbag")
            added = true;
        end;
        -- dont let player die
        player:SetMod(modifiersGlobal.MinimumHpLock, 1);
		contentGroup:AddMember(player)
        print("shittttt")
		break
    end;
    print("shit")
	contentGroup:AddMember(director);
    print("shit2");
	contentGroup:AddMember(yshtola);
    print("shit3")
	contentGroup:AddMember(stahlmann);
	print("shit4")
    contentGroup:AddMember(mob1);
	print("shit5")
    contentGroup:AddMember(mob2);
	print("shit6")
    contentGroup:AddMember(mob3);
    print("dicks")
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
	print("ass")
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
    print("fuck")
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
    print("shitstain")
    onCreateContentArea(director:GetPlayerMembers(), director, director:GetZone(), contentGroup);
    player:EndEvent();
end;