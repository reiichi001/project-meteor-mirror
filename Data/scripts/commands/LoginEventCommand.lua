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
		warpOutOfInn(player, innCode, true);		
	end
	player:EndEvent();
	
end

function warpOutOfInn(player, innCode, isNightmare)
	local spawnCode = SPAWN_NO_ANIM;

	if (isNightmare) then
		spawnCode = SPAWN_NIGHTMARE;
	end

	if (innCode == 1) then		
		GetWorldManager():DoZoneChange(player, 133, nil, 0, spawnCode, -444.266, 39.518, 191, 1.9);
	elseif (innCode == 2) then
		GetWorldManager():DoZoneChange(player, 155, nil, 0, spawnCode, 59.252, 4, -1219.342, 0.852);
	elseif (innCode == 3) then
		GetWorldManager():DoZoneChange(player, 209, nil, 0, spawnCode, -110.157, 202, 171.345, 0);
	end
end