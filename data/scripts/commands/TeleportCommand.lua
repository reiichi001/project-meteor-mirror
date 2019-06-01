--[[

TeleportCommand Script

Functions:

eventRegion(numAnima)
eventAetheryte(region, animaCost1, animaCost2, animaCost3, animaCost4, animaCost5, animaCost6)
eventConfirm(isReturn, isInBattle, cityReturnNum, 138821, forceAskReturnOnly)

--]]

require ("global")
require ("aetheryte")
require ("utils")

teleportMenuToAetheryte = {
	[1] = {
		[1] = 1280001,
		[2] = 1280002,
		[3] = 1280003,
		[4] = 1280004,
		[5] = 1280005,
		[6] = 1280006
	},
	[2] = {
		[1] = 1280092,
		[2] = 1280093,
		[3] = 1280094,
		[4] = 1280095,
		[5] = 1280096
	},
	[3] = {
		[1] = 1280061,
		[2] = 1280062,
		[3] = 1280063,
		[4] = 1280064,
		[5] = 1280065,
		[6] = 1280066
	},
	[4] = {
		[1] = 1280031,
		[2] = 1280032,
		[3] = 1280033,
		[4] = 1280034,
		[5] = 1280035,
		[6] = 1280036
	},
	[5] = {
		[1] = 1280121,
		[2] = 1280122		
	}
}

function onEventStarted(player, actor, triggerName, isTeleport)

	local worldMaster = GetWorldMaster();

	if (isTeleport == 0) then		
		while (true) do
			regionChoice = callClientFunction(player, "delegateCommand", actor, "eventRegion", 100);
			
			if (regionChoice == nil) then break	end
		
			while (true) do
				aetheryteChoice = callClientFunction(player, "delegateCommand", actor, "eventAetheryte", regionChoice, 2, 2, 2, 4, 4, 4);
				
				if (aetheryteChoice == nil) then break end
				
				player:PlayAnimation(0x4000FFA);
				player:SendGameMessage(worldMaster, 34101, 0x20, 2, teleportMenuToAetheryte[regionChoice][aetheryteChoice], 100, 100);	
				confirmChoice = callClientFunction(player, "delegateCommand", actor, "eventConfirm", false, false, 1, 138824, false);				
				if (confirmChoice == 1) then
					player:PlayAnimation(0x4000FFB);
					player:SendGameMessage(worldMaster, 34105, 0x20);			
					--Do teleport		
					destination = aetheryteTeleportPositions[teleportMenuToAetheryte[regionChoice][aetheryteChoice]];			
					if (destination ~= nil) then						
						randoPos = getRandomPointInBand(destination[2], destination[4], 3, 5);
						rotation = getAngleFacing(randoPos.x, randoPos.y, destination[2], destination[4]);
						GetWorldManager():DoZoneChange(player, destination[1], nil, 0, 2, randoPos.x, destination[3], randoPos.y, rotation);
					end
				end
				player:endEvent();
				return;
			end
			
		end
	else
		player:PlayAnimation(0x4000FFA);
		local choice, isInn = callClientFunction(player, "delegateCommand", actor, "eventConfirm", true, false, player:GetHomePointInn(), player:GetHomePoint(), false);		
		if (choice == 1) then
			player:PlayAnimation(0x4000FFB);
			player:SendGameMessage(worldMaster, 34104, 0x20);

			--bandaid fix for returning while dead, missing things like weakness and the heal number
			if (player:GetHP() == 0) then
				player:SetHP(player.GetMaxHP());
				player:ChangeState(0);
				player:PlayAnimation(0x01000066);
			end

			if (isInn) then
				--Return to Inn		
				if (player:GetHomePointInn() == 1) then
					GetWorldManager():DoZoneChange(player, 244, nil, 0, 15, -160.048, 0, -165.737, 0);
				elseif (player:GetHomePointInn() == 2) then
					GetWorldManager():DoZoneChange(player, 244, nil, 0, 15, 160.048, 0, 154.263, 0);
				elseif (player:GetHomePointInn() == 3) then
					GetWorldManager():DoZoneChange(player, 244, nil, 0, 15, 0.048, 0, -5.737, 0);
				end			
			elseif (choice == 1 and isInn == nil) then			
				--Return to Homepoint
				destination = aetheryteTeleportPositions[player:GetHomePoint()];
				if (destination ~= nil) then
					randoPos = getRandomPointInBand(destination[2], destination[4], 3, 5);
					rotation = getAngleFacing(randoPos.x, randoPos.y, destination[2], destination[4]);
					GetWorldManager():DoZoneChange(player, destination[1], nil, 0, 2, randoPos.x, destination[3], randoPos.y, rotation);

				end
			end
		end
	end	

	player:endEvent();
end