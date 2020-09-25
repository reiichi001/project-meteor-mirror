require ("global")

--[[

CmnAttackWeaponSkill Script

Finds the correct weaponskill subscript to fire when a weaponskill actor is activated.

--]]

local function handleTEST(player)
end

local weaponskillHandlers = {
	[0xA0F067F4] = nil,
	[0xA0F067F5] = nil,
	[0xA0F067F7] = nil,
	[0xA0F067F8] = nil,
	[0xA0F067FA] = nil,
	[0xA0F067FB] = nil,
	[0xA0F067FD] = nil,
	[0xA0F067FE] = nil,
	[0xA0F06800] = nil,
	[0xA0F06801] = nil,
	[0xA0F06802] = nil,
	[0xA0F06804] = nil,
	[0xA0F06805] = nil,
	[0xA0F06806] = nil,
	[0xA0F06808] = nil,
	[0xA0F0680A] = nil,
	[0xA0F0680B] = nil,
	[0xA0F0680D] = nil,
	[0xA0F0680E] = nil,
	[0xA0F06810] = nil,
	[0xA0F06812] = nil,
	[0xA0F06814] = nil,
	[0xA0F068A8] = nil,
	[0xA0F068A9] = nil,
	[0xA0F068AB] = nil,
	[0xA0F068AC] = nil,
	[0xA0F068AE] = nil,
	[0xA0F068AF] = nil,
	[0xA0F068B2] = nil,
	[0xA0F068B3] = nil,
	[0xA0F068B5] = nil,
	[0xA0F068B7] = nil,
	[0xA0F068B8] = nil,
	[0xA0F068B9] = nil,
	[0xA0F068BB] = nil,
	[0xA0F068BC] = nil,
	[0xA0F068BE] = nil,
	[0xA0F068BF] = nil,
	[0xA0F068C1] = nil,
	[0xA0F068C3] = nil,
	[0xA0F068C5] = nil,
	[0xA0F0695C] = nil,
	[0xA0F0695D] = nil,
	[0xA0F0695E] = nil,
	[0xA0F06960] = nil,
	[0xA0F06961] = nil,
	[0xA0F06963] = nil,
	[0xA0F06964] = nil,
	[0xA0F06966] = nil,
	[0xA0F06967] = nil,
	[0xA0F06968] = nil,
	[0xA0F0696A] = nil,
	[0xA0F0696B] = nil,
	[0xA0F0696D] = nil,
	[0xA0F0696E] = nil,
	[0xA0F06970] = nil,
	[0xA0F06971] = nil,
	[0xA0F06973] = nil,
	[0xA0F06974] = nil,
	[0xA0F06976] = nil,
	[0xA0F06978] = nil,
	[0xA0F06B78] = nil,
	[0xA0F06B79] = nil,
	[0xA0F06B7B] = nil,
	[0xA0F06B7C] = nil,
	[0xA0F06B7E] = nil,
	[0xA0F06B7F] = nil,
	[0xA0F06B81] = nil,
	[0xA0F06B82] = nil,
	[0xA0F06B84] = nil,
	[0xA0F06B85] = nil,
	[0xA0F06B8A] = nil,
	[0xA0F06B8C] = nil,
	[0xA0F06B8E] = nil,
	[0xA0F06B90] = nil,
	[0xA0F06B91] = nil,
	[0xA0F06B93] = nil,
	[0xA0F06B95] = nil,
	[0xA0F06B97] = nil,
	[0xA0F06C2C] = nil,
	[0xA0F06C2D] = nil,
	[0xA0F06C2F] = nil,
	[0xA0F06C31] = nil,
	[0xA0F06C32] = nil,
	[0xA0F06C34] = nil,
	[0xA0F06C35] = nil,
	[0xA0F06C36] = nil,
	[0xA0F06C38] = nil,
	[0xA0F06C39] = nil,
	[0xA0F06C3B] = nil,
	[0xA0F06C3C] = nil,
	[0xA0F06C3E] = nil,
	[0xA0F06C3F] = nil,
	[0xA0F06C41] = nil,
	[0xA0F06C43] = nil,
	[0xA0F06C45] = nil,
	[0xA0F06C47] = nil,
	[0xA0F06C49] = nil,
	[0xA0F06C4B] = nil,
	[0xA0F06D94] = nil,
	[0xA0F06D95] = nil,
	[0xA0F06D96] = nil,
	[0xA0F06F92] = nil,
	[0xA0F06F93] = nil,
	[0xA0F06F95] = nil,
	[0xA0F06F96] = nil,
	[0xA0F070E6] = nil,
	[0xA0F070E7] = nil,
	[0xA0F070E9] = nil,
	[0xA0F070EA] = nil
}

function onEventStarted(player, command, triggerName)			
	
	--Are they in active mode?
	if (player:GetState() != 2) then
		player:SendGameMessage(GetWorldMaster(), 32503, 0x20);
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