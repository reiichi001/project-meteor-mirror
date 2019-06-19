--[[

BonusPointCommand Script

Functions:

operateUI(pointsAvailable, pointsLimit, str, vit, dex, int, min, pie)

--]]

require ("global")

function onEventStarted(player, actor, triggerName)
	--local points = player:GetAttributePoints();
	--player:RunEventFunction("delegateCommand", actor, "operateUI", points.available, points.limit, points.inSTR, points.inVIT, points.inDEX, points.inINT, points.inMIN, points.inPIT);
	result = callClientFunction(player, "delegateCommand", actor, "operateUI", 100, 100, 10, 10, 10, 10, 10, 10);
	
	--Do Save
	if (result == true) then
	end
	
	player:endEvent();
end