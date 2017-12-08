require("global")

function onEventStarted(player, command, triggerName, arg1, arg2, arg3, arg4, targetActor, arg5, arg6, arg7, arg8)
	
    local worldManager = GetWorldManager();
    --local shortCommandId = command.actorId;--bit32:bxor(command.actorId, 2700083200);
    local ability = worldManager:GetAbility(command.actorId);

    if ability then
        player.SendBattleActionX01Packet(ability.modelAnimation, ability.effectAnimation, 0x756D, command.actorId, ability.animationType);
    end

    player:endEvent();
end