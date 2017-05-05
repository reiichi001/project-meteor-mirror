require("global");

properties = {
    permissions = 0,
    parameters = "ssss",
    description = ""
}

function onTrigger(player, argc, actorClassId, regionId, layoutId, maxLayoutId)
	layoutId = tonumber(layoutId);
	
	if (maxLayoutId ~= nil) then
		maxLayoutId = tonumber(maxLayoutId);
	else
		maxLayoutId = layoutId;
	end
	
	while (layoutId <= maxLayoutId) do
		if (actorClassId == nil) then
			player:SendMessage(0x20, "", "No actor class id provided.");
			return;
		end	

		local pos = player:GetPos();
		local x = pos[0];
		local y = pos[1];
		local z = pos[2];
		local zone = pos[4];
			 
		actorClassId = tonumber(actorClassId);
		
		if (actorClassId ~= nil) then		
			zone = player:GetZone();
			actor = zone:SpawnActor(actorClassId, "mapobj", pos[0], pos[1], pos[2], tonumber(regionId), tonumber(layoutId));
			wait(0.8);
			actor:PlayMapObjAnimation(player, "open");
			zone:DespawnActor("mapobj");
			wait(0.5);
			player:SendMessage(0x20, "", "Layout ID: "..layoutId);
		end
		layoutId=layoutId+1;
	end
end;
--!testmapobj 5900001 421 2810 2820

--dun4 (0x19e) - Copperbell Mines
--dun1 - Mun Tuy
--dun2 - Tam Tara

--Ferry (Thanalan, Z=-130): 5145, 252
--Ferry (La Noscea, Z=+130): 5144, 201
--Ferry (5142, Z=-130) Doors: 323, 326, 
--Ferry (5143, Z=+130) Doors: 323, 326, 