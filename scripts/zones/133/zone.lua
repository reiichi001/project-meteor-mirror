function onZoneInit(zone)
end

function onZoneIn(player)
	player:sendMessage(0x1D,"",">Callback \"onZoneIn\" for zone 133 running.");
end

function onZoneOut(zone, player)
end