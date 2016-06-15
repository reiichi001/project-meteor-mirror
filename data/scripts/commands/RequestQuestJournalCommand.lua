--[[


--]]

function onEventStarted(player, actor, questId)
	player:SendRequestedInfo("requestedData", "qtdata", 0x1D4F2);
end
