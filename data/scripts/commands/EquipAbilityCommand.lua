require ("global")
--player: Player that called this command
--equipAbilityWidget: Widget that calls this command
--triggername: Event Starter ?
--slot: Which slot the ability will go into
--ability: Ability being equipped


function onEventStarted(player, equipAbilityWidget, triggername, slot, ability, unkown, arg1, arg2, arg3, arg4, arg5, arg6)
    if ability then
        player:EquipAbility(slot, ability, 1);
    end
	player:endEvent();	
end