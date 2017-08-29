require ("global")
--player: Player that called this command
--equipAbilityWidget: Widget that calls this command
--triggername: Event Starter ?
--slot: Which slot the ability will go into
--commandid: command being equipped


function onEventStarted(player, equipAbilityWidget, triggername, slot, commandid, unkown, arg1, arg2, arg3, arg4, arg5, arg6)
    local worldManager = GetWorldManager();
    local ability = worldManager:GetBattleCommand(commandid);
    
    --Equip
    if (commandid > 0) then
        --Can the player equip any more cross class actions
        if (player.charaWork.parameterTemp.otherClassAbilityCount[0] >= player.charaWork.parameterTemp.otherClassAbilityCount[1]) then
            --"You cannot set any more actions."
            player:SendGameMessage(GetWorldMaster(), 30720, 0x20, 0, 0);	    
            player:endEvent();
            return;
        end

        --Is the player high enough level in that class to equip the ability
        if (player.charaWork.battleSave.skillLevel[ability.job] < ability.level) then

            --"You have not yet acquired that action"
            player:SendGameMessage(GetWorldMaster(), 30742, 0x20, 0, 0);
            player:endEvent();
            return;
        end

        --Equip the ability
        player:EquipAbility(slot, commandid);
    --Unequip
    elseif (commandid == 0) then
        commandid = player.charaWork.command[slot + player.charaWork.commandBorder];

        --Is the ability a part of the player's current class?
        --This check isn't correct because of jobs having different ids
        if(worldManager:GetBattleCommand(commandid).job == player.charaWork.parameterSave.state_mainSkill[0]) then
                --"Actions of your current class or job cannot be removed."
                player:SendGameMessage(GetWorldMaster(), 30745, 0x20, 0, 0);
        elseif (commandid != 0) then
            player:UnequipAbility(slot);
        end
    end
	player:endEvent();	
end