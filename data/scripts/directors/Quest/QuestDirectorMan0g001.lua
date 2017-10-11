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
    --[[
    --yda = GetWorldManager().SpawnBattleNpcById(6, contentArea);
    --papalymo = GetWorldManager().SpawnBattleNpcById(7, contentArea);
    --yda:ChangeState(2);
    mob1 = GetWorldManager().SpawnBattleNpcById(3, contentArea);
    mob2 = GetWorldManager().SpawnBattleNpcById(4, contentArea);
    mob3 = GetWorldManager().SpawnBattleNpcById(5, contentArea);

    --papalymo = contentArea:SpawnActor(2290005, "papalymo", 365.89, 4.0943, -706.72, -0.718);
	--yda = contentArea:SpawnActor(2290006, "yda", 365.266, 4.122, -700.73, 1.5659);	
	--yda:ChangeState(2);
	
	--mob1 = contentArea:SpawnActor(2201407, "mob1", 374.427, 4.4, -698.711, -1.942);
	--mob2 = contentArea:SpawnActor(2201407, "mob2", 375.377, 4.4, -700.247, -1.992);
	--mob3 = contentArea:SpawnActor(2201407, "mob3", 375.125, 4.4, -703.591, -1.54);
	
	openingStoper = contentArea:SpawnActor(1090384, "openingstoper", 356.09, 3.74, -701.62, -1.41);
	
    
    local added = false;
    for player in players do
        if player.currentParty and not added then
            player.currentParty.members:Add(yda.actorId);
            print("cunt");
            player.currentParty.members:Add(papalymo.actorId);
            print("dickbag");
            added = true;
        end;
        -- dont let player die
        print("shittttt3");
        player:SetMod(modifiersGlobal.MinimumHpLock, 1);
        print("shittttt2");
        director:AddMember(player)
        print("shittttt1");
    end;
    print("shit")
    director:AddMember(director);
    director:AddMember(yda);
    director:AddMember(papalymo);
    director:AddMember(mob1);
    director:AddMember(mob2);
    print("shit6")
    director:AddMember(mob3);
    print("dicks")
]]
	director:StartContentGroup();
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
end;