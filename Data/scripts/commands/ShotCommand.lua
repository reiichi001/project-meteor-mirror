require ("global")
require ("utils")

--[[

AttackWeaponSkill Script

Finds the correct weaponskill subscript to fire when a weaponskill actor is activated.

--]]

function onEventStarted(player, command, triggerName, arg1, arg2, arg3, arg4, targetActor, arg5, arg6, arg7, arg8)
	player.Ability(command.actorId, targetActor);
	player:endEvent();
end;