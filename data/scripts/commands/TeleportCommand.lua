--[[

TeleportCommand Script

Functions:

eventRegion(numAnima)
eventAetheryte(region, animaCost1, animaCost2, animaCost3, animaCost4, animaCost5, animaCost6)
eventConfirm(isReturn, isInBattle, cityReturnNum, 138821, forceAskReturnOnly)

--]]

require ("global")

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
				player:SendGameMessage(worldMaster, 34101, 0x20, 2, 0x13883d, 100, 100);	
				confirmChoice = callClientFunction(player, "delegateCommand", actor, "eventConfirm", false, false, 1, 138824, false);				
				if (confirmChoice == 1) then
					player:PlayAnimation(0x4000FFB);
					player:SendGameMessage(worldMaster, 34105, 0x20);			
					--Do teleport					
				end
				
				player:endEvent();
				return;
			end
			
		end
	else
		player:PlayAnimation(0x4000FFA);
		local choice, wasMulti = callClientFunction(player, "delegateCommand", actor, "eventConfirm", true, false, player:GetHomePointInn(), player:GetHomePoint(), false);		
		if (wasMulti and choice == 1 or choice == 2) then
			player:PlayAnimation(0x4000FFB);
			player:SendGameMessage(worldMaster, 34104, 0x20);
			--Do return
		elseif (not wasMulti and choice == 1) then
			player:PlayAnimation(0x4000FFB);
			player:SendGameMessage(worldMaster, 34104, 0x20);
			--Do return
		end
	end	

	player:endEvent();
end