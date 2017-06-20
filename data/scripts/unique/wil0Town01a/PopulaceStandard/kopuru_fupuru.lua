require ("global")

function onEventStarted(player, npc)
	defaultWil = GetStaticActor("DftWil");	
	choice = callClientFunction(player, "delegateEvent", player, defaultWil, "defaultTalkWithInn_Desk_2", nil, nil, nil);
	
	if (choice == 1) then
		GetWorldManager():DoZoneChange(player, 244, nil, 0, 15, 0.048, 0, -5.737, 0);
	elseif (choice == 2) then
		if (player:GetHomePointInn() ~= 3) then
			player:SetHomePointInn(3);
			player:SendGameMessage(GetWorldMaster(), 60019, 0x20, 3071); --Secondary homepoint set to the Hourglass
		else			
			player:SendGameMessage(GetWorldMaster(), 51140, 0x20); --This inn is already your Secondary Homepoint
		end
	end
	
	player:EndEvent();	
end