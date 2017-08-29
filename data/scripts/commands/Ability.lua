require ("global")
require ("utils")

--[[

AttackWeaponSkill Script

Finds the correct weaponskill subscript to fire when a weaponskill actor is activated.

--]]

local attackMagicHandlers = {
    
}

function onEventStarted(player, command, triggerName, arg1, arg2, arg3, arg4, targetActor, arg5, arg6, arg7, arg8)
	print(command.actorId)
	--Are they in active mode?
	if (player:GetState() != 2) then
		player:SendGameMessage(GetWorldMaster(), 32503, 0x20);
		player:endEvent();
		return;
	end

	player.Ability(command.actorId, targetActor);
	player:endEvent();
	
end