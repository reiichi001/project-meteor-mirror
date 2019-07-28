--[[

LoginEventCommand Script

Handles post-dream events.

--]]

require ("global")

function onEventStarted(player, actor, triggerName, dreamCode, innCode, narg1, narg2, bedActor)
	
	--In Plain Sight
	if (dreamCode == 1) then
		player:AddQuest("Etc5g1");
		warpOutOfInn(player, innCode);
	--Prophecy Inspection
	elseif (dreamCode == 2) then
		player:AddQuest("Etc5l3");
		warpOutOfInn(player, innCode);
	--Nael Nightmare
	elseif (dreamCode == 20) then		
		GetWorldManager():DoZoneChange(player, 244, nil, 0, SPAWN_NIGHTMARE, player.positionX, player.positionY, player.positionZ, player.rotation);		
	end
	player:EndEvent();
	
end

function warpOutOfInn(player, innCode)
	if (innCode == 1) then		
		GetWorldManager():DoZoneChange(player, 133, nil, 0, 15, -444.266, 39.518, 191, 1.9);
	elseif (innCode == 2) then
		GetWorldManager():DoZoneChange(player, 155, nil, 0, 15, 59.252, 4, -1219.342, 0.852);
	elseif (innCode == 3) then
		GetWorldManager():DoZoneChange(player, 209, nil, 0, 15, -110.157, 202, 171.345, 0);
	end
end