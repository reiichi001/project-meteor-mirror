--[[

TeleportCommand Script

Functions:

eventRegion(numAnima)
eventAetheryte(region, animaCost1, animaCost2, animaCost3, animaCost4, animaCost5, animaCost6)
eventConfirm(isReturn, isInBattle, cityReturnNum, 138821, forceAskReturnOnly)

--]]

require ("global")

function doTeleport(region, aetheryte)
end

function onEventStarted(player, actor, triggerName, isTeleport)
	if (isTeleport == 0) then		
		while (true) do
			regionChoice = callClientFunction(player, "delegateCommand", actor, "eventRegion", 100);
			
			if (regionChoice == nil) then break	end
		
			while (true) do
				aetheryteChoice = callClientFunction(player, "delegateCommand", actor, "eventAetheryte", regionChoice, 2, 2, 2, 4, 4, 4);
				
				if (aetheryteChoice == nil) then break end
				
				confirmChoice = callClientFunction(player, "delegateCommand", actor, "eventConfirm", false, false, 1, 138824, false);
				
				if (confirmChoice == 1) then
					doTeleport(regionChoice, aetheryteChoice);								
				end
				
				player:endEvent();
				return;
								
			end
			
		end
	else
		callClientFunction(player, "delegateCommand", actor, "eventConfirm", true, false, 1, 0x138824, false);
	end	

	player:endEvent();
end

function onEventUpdate(player, actor, step, arg1)

	menuId = player:GetCurrentMenuId();
	
	if (menuId == 0) then --Region
		if (arg1 ~= nil and arg1 >= 1) then
			player:SetCurrentMenuId(1);
			player:RunEventFunction("delegateCommand", actor, "eventAetheryte", arg1, 2, 2, 2, 4, 4, 4);
		else
			player:EndCommand();
		end
	elseif (menuId == 1) then --Aetheryte
		if (arg1 == nil) then
			player:EndCommand();
			return;
		end
		player:SetCurrentMenuId(2);
		player:RunEventFunction("delegateCommand", actor, "eventConfirm", false, false, 1, 138824, false);
	elseif (menuId == 2) then --Confirm
		player:EndCommand();	
	end
	
end
