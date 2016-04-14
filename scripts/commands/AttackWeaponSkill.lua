--[[

AttackWeaponSkill Script

Finds the correct weaponskill subscript to fire when a weaponskill actor is activated.

--]]



function onEventStarted(player, actor, triggerName)	

	worldMaster = getWorldMaster();

	if (player:getState() != 2) then
		player:sendGameMessage(worldMaster, 32503, 0x20);
	end

	player:endCommand();
	
end

function onEventUpdate(player, npc)
end