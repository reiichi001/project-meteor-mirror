require ("global")

function onEventStarted(player, npc)
	defaultSea = GetStaticActor("DftSea");
	choice = callClientFunction(player, "delegateEvent", player, defaultSea, "defaultTalkWithInn_Desk", nil, nil, nil);
	
	if (choice == 1) then
		GetWorldManager():DoZoneChange(player, 244, nil, 0, 15, -160.048, 0, -165.737, 0);
	elseif (choice == 2) then
		if (player:GetHomePointInn() ~= 1) then
			player:SetHomePointInn(1);
			player:SendGameMessage(GetWorldMaster(), 60019, 0x20, 1070); --Secondary homepoint set to the Mizzenmast
		else			
			player:SendGameMessage(GetWorldMaster(), 51140, 0x20); --This inn is already your Secondary Homepoint
		end
	end
	
	player:EndEvent();	
end