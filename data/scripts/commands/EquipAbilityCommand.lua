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
        --[[]]
        --Can the player equip any more cross class actions
        if (player.charaWork.parameterTemp.otherClassAbilityCount[0] >= player.charaWork.parameterTemp.otherClassAbilityCount[1]) then
            --"You cannot set any more actions."
            player:SendGameMessage(GetWorldMaster(), 30720, 0x20, 0, 0);	    
            player:endEvent();
            return;
        end
            
        --Is the player high enough level in that class to equip the ability
        if (player.charaWork.battleSave.skillLevel[ability.job - 1] < ability.level) then
            --"You have not yet acquired that action."
            player:SendGameMessage(GetWorldMaster(), 30742, 0x20, 0, 0);
            player:endEvent();
            return;
        end

        
        local oldSlot = player:FindFirstCommandSlotById(commandid);
        local isEquipped = oldSlot < player.charaWork.commandBorder + 30;
        --If slot is 0, find the first open slot
        if (slot == 0) then
            --If the ability is already equipped and slot is 0, then it can't be equipped again
            --If the slot isn't 0, it's a move or a swap command
            if (isEquipped == true) then
                --"That action is already set to an action slot."
                player:SendGameMessage(GetWorldMaster(), 30719, 0x20, 0);
                player:endEvent();
                return;
            end

            slot = player:FindFirstCommandSlotById(0) - player.charaWork.commandBorder;

            --If the first open slot is outside the hotbar, then the hotbar is full
            if(slot >= 30) then
                --"You cannot set any more actions."
                player:SendGameMessage(Server.GetWorldManager().GetActor(), 30720, 0x20, 0);
                player:endEvent();
                return;
            end
        else
            slot = slot - 1;
        end

        if(isEquipped == true) then
            player:SwapAbilities(oldSlot, slot + player.charaWork.commandBorder);
        else
            local tslot = slot + player.charaWork.commandBorder;
            player:EquipAbility(player.GetCurrentClassOrJob(), commandid, tslot, true);
        end

    --Unequip
    elseif (commandid == 0) then
        commandid = player.charaWork.command[slot + player.charaWork.commandBorder - 1];
        ability = worldManager.GetBattleCommand(commandid);
        --Is the ability a part of the player's current class?
        --This check isn't correct because of jobs having different ids
        local classId = player:GetCurrentClassOrJob();
        local jobId = player:ConvertClassIdToJobId(classId);

        if(ability.job == classId or ability.job == jobId) then
            --"Actions of your current class or job cannot be removed."
            player:SendGameMessage(GetWorldMaster(), 30745, 0x20, 0, 0);
        elseif (commandid != 0) then
            player:UnequipAbility(slot);
        end
    end

	player:endEvent();	
end