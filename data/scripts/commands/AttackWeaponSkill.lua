require ("global")
require ("utils")

--[[

AttackWeaponSkill Script

Finds the correct weaponskill subscript to fire when a weaponskill actor is activated.

--]]

local function handlePummel(player, target)
	player:SendMessage(0x20, "", "DOING PUMMEL!!!!");
	
	params = {};
	params.range = 10.0;
	params.recast = 10;
	
	params.hpCost = 0;
	params.mpCost = 0;
	params.tpCost = 1000;
	
	params.targetType = 2;
	params.canCrit = true;
	params.animationId = 0x12312312;	
	
	
end

local function handleSkullSunder(player)
	player:SendMessage(0x20, "", "DOING SKULL SUNDER!!!!");
end

local weaponskillHandlers = {
	[0xA0F069E6] = handlePummel,
	[0xA0F069E7] = nil,
	[0xA0F069E8] = nil,
	[0xA0F069E9] = nil,
	[0xA0F069EA] = nil,
	[0xA0F069EB] = nil,
	[0xA0F069EC] = nil,
	[0xA0F069ED] = nil,
	[0xA0F069EE] = nil,
	[0xA0F069EF] = nil,
	[0xA0F06A0E] = nil,
	[0xA0F06A0F] = nil,
	[0xA0F06A10] = nil,
	[0xA0F06A11] = nil,
	[0xA0F06A12] = nil,
	[0xA0F06A13] = nil,
	[0xA0F06A14] = nil,
	[0xA0F06A15] = nil,
	[0xA0F06A16] = nil,
	[0xA0F06A17] = nil,
	[0xA0F06A36] = nil,
	[0xA0F06A37] = handleSkullSunder,
	[0xA0F06A38] = nil,
	[0xA0F06A39] = nil,
	[0xA0F06A3A] = nil,
	[0xA0F06A3B] = nil,
	[0xA0F06A3C] = nil,
	[0xA0F06A3D] = nil,
	[0xA0F06A3E] = nil,
	[0xA0F06A3F] = nil,
	[0xA0F06A5C] = nil,
	[0xA0F06A5D] = nil,
	[0xA0F06A5E] = nil,
	[0xA0F06A60] = nil,
	[0xA0F06A61] = nil,
	[0xA0F06A62] = nil,
	[0xA0F06A63] = nil,
	[0xA0F06A64] = nil,
	[0xA0F06A85] = nil,
	[0xA0F06A86] = nil,
	[0xA0F06A87] = nil,
	[0xA0F06A88] = nil,
	[0xA0F06A89] = nil,
	[0xA0F06A8A] = nil,
	[0xA0F06A8B] = nil,
	[0xA0F06A8C] = nil,
	[0xA0F06A8D] = nil,
	[0xA0F06A8E] = nil,
	[0xA0F06A8F] = nil
}

function onEventStarted(player, command, triggerName, arg1, arg2, arg3, arg4, targetActor, arg5, arg6, arg7, arg8)
	
	--Are they in active mode?
	if (player:GetState() != 2) then
		player:SendGameMessage(GetWorldMaster(), 32503, 0x20);
		player:endEvent();
		return;
	end
	
	--Does the target exist
	target = player:getZone():FindActorInArea(targetActor);
	if (target == nil) then
		player:SendGameMessage(GetWorldMaster(), 30203, 0x20);
		player:endEvent();
		return;
	end
	
	--Are you too far away?
	if (getDistanceBetweenActors(player, target) > 7) then
		player:SendGameMessage(GetWorldMaster(), 32539, 0x20);
		player:endEvent();
		return;
	end	
	
	if (weaponskillHandlers[command.actorId] ~= nil) then
		weaponskillHandlers[command.actorId](player);
	else
		player:SendMessage(0x20, "", "That weaponskill is not implemented yet.");
	end	
	
	player:endEvent();	
	
end