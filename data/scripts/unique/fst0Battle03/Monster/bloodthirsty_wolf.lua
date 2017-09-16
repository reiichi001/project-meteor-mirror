require ("global")
require ("modifiers")
require ("ally")

function onSpawn(mob)

end;

function onDamageTaken(mob, attacker, damage, damageType)
    if attacker.IsPlayer() then
        local man0g0Quest = attacker:GetQuest("Man0g0");
        if damageType == DAMAGE_TAKEN_TYPE_ATTACK then
            if man0g0Quest:GetPhase() == 5 then
                closeTutorialWidget(player);
            	showTutorialSuccessWidget(player, 9055); --Open TutorialSuccessWidget for attacking enemy
                man0g0Quest:NextPhase(6);
            end;
        elseif damageType == DAMAGE_TAKEN_TYPE_WEAPONSKILL or damageType == DAMAGE_TAKEN_TYPE_MAGIC then
            if man0g0Quest:GetPhase() == 6 then
                closeTutorialWidget(player);
                showTutorialSuccessWidget(player, 9065); --Open TutorialSuccessWidget for weapon skill
                man0g0Quest:NextPhase(7);
            end;
        end;
    end;
end;

function onDeath(mob, player, lastAttacker)
    if player then
        local man0g0Quest = player:GetQuest("Man0g0");
        if man0g0Quest and man0g0Quest:GetPhase() >= 7 then
            man0g0Quest:NextPhase(man0g0Quest:GetPhase() + 1);
            mob:SetTempVar("playerId", player.actorId);
            if man0g0Quest:GetPhase() == 10 then
            	local worldMaster = GetWorldMaster();
                player:SendDataPacket("attention", worldMaster, "", 51073, 1);
                kickEventContinue(player, director, "noticeEvent", "noticeEvent");
                callClientFunction(player, "delegateEvent", player, man0g0Quest, "processEvent020_1", nil, nil, nil);	
                player:ChangeMusic(7);
                player:Disengage(0x0000);
                mob:SetTempVar("complete", 1);
            end;
        end;
    end;
end;

function onDespawn(mob)
    if zone then
        local player = zone.FindActorInArea(mob:GetTempVar("playerId"));
        
        if player and mob:GetTempVar("complete") == 1 then
            local man0g0Quest = player:GetQuest("Man0g0");
            player:GetZone():ContentFinished();
            player:EndEvent();
            GetWorldManager():DoZoneChange(player, 155, "PrivateAreaMasterPast", 1, 15, 175.38, -1.21, -1156.51, -2.1);
        end;
    end;
end;