--[[

TeleportCommand Script

Functions:

eventRegion(numAnima)
eventAetheryte(region, animaCost1, animaCost2, animaCost3, animaCost4, animaCost5, animaCost6)
eventConfirm(isReturn, isInBattle, cityReturnNum, 138821, forceAskReturnOnly)

Menu Ids:

Region Menu:	0
Aetheryte Menu:	1
Confirm Menu:	2

--]]

function onEventStarted(player, actor, triggerName, isTeleport)
	if (isTeleport == 0) then
		player:setCurrentMenuId(0);
		player:runEventFunction("delegateCommand", actor, "eventRegion", 100);
	else
		player:setCurrentMenuId(2);
		player:runEventFunction("delegateCommand", actor, "eventConfirm", true, false, 1, 0x138824, false);
	end		
end

function onEventUpdate(player, actor, step, arg1)

	menuId = player:getCurrentMenuId();
	
	if (menuId == 0) then --Region
		if (arg1 ~= nil and arg1 >= 1) then
			player:setCurrentMenuId(1);
			player:runEventFunction("delegateCommand", actor, "eventAetheryte", arg1, 2, 2, 2, 4, 4, 4);
		else
			player:endCommand();
		end
	elseif (menuId == 1) then --Aetheryte
		if (arg1 == nil) then
			player:endCommand();
			return;
		end
		player:setCurrentMenuId(2);
		player:runEventFunction("delegateCommand", actor, "eventConfirm", false, false, 1, 138824, false);
	elseif (menuId == 2) then --Confirm
		player:endCommand();	
	end
	
end