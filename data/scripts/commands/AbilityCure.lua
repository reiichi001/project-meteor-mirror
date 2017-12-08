require("global")

function onEventStarted(player, command, triggerName, arg1, arg2, arg3, arg4, targetActor, arg5, arg6, arg7, arg8)
	
    local worldManager = GetWorldManager();
    local shortCommandId = bit32.bxor(command, 2700083200);
    local ability = worldManager:GetAbility(shortCommandId);

    --player:PlayAnimation(ability.modelAnimation);


    player:endEvent();
end