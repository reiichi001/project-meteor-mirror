--[[

BonusPointCommand Script

Functions:

operateUI(pointsAvailable, pointsLimit, str, vit, dex, int, min, pie)

--]]

function onEventStarted(player, actor, triggerName)
	--local points = player:getAttributePoints();
	--player:runEventFunction("delegateCommand", actor, "operateUI", points.available, points.limit, points.inSTR, points.inVIT, points.inDEX, points.inINT, points.inMIN, points.inPIT);
	player:runEventFunction("delegateCommand", actor, "operateUI", 10, 10, 10, 10, 10, 10, 10, 10);
end

function onEventUpdate(player, actor, step, arg1)

	--Submit
	
	player:endCommand();
	
end