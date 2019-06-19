function onEventStarted(player, command, triggerName, arg1, arg2, arg3, arg4, targetActor, arg5, arg6, arg7, arg8)
	player.Cast(command.actorId, targetActor);
    
	player:endEvent();
end