--[[

AttackWeaponSkill Script

Finds the correct weaponskill subscript to fire when a weaponskill actor is activated.

--]]



function onEventStarted(player, actor, triggerName)	

	worldMaster = GetWorldMaster();

	if (player:GetState() != 2) then
		player:SendGameMessage(worldMaster, 32503, 0x20);
	end

	player:EndEvent();
	
end

function onEventUpdate(player, npc)
end