require ("global")
require ("utils")

--[[

AttackWeaponSkill Script

Finds the correct weaponskill subscript to fire when a weaponskill actor is activated.

--]]

function onEventStarted(player, command, triggerName, arg1, arg2, arg3, arg4, targetActor, arg5, arg6, arg7, arg8)
	
	--Are they in active mode?
	if (player:GetState() != 2) then
		player:SendGameMessage(GetWorldMaster(), 32503, 0x20);
		player:endEvent();
		return;
	end
	
    if not player.aiContainer.IsEngaged() then
        player.Engage(targetActor);
    end;
    player.WeaponSkill(command.actorId, targetActor);
	player:endEvent();
end;