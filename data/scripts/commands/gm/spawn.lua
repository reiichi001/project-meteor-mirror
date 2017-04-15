require("global");

properties = {
    permissions = 0,
    parameters = "d",
    description = "Spawns a actor",
}

function onTrigger(player, argc, actorClassId)

	if (actorClassId == nil) then
		player:SendMessage(0x20, "", "No actor class id provided.");
		return;
	end	

    local pos = player:GetPos();
    local x = pos[0];
    local y = pos[1];
    local z = pos[2];
    local rot = pos[3];
    local zone = pos[4];
         
	actorClassId = tonumber(actorClassId);
	
	if (actorClassId ~= nil) then		
		zone = player:GetZone();
		actor = zone:SpawnActor(actorClassId, "test", pos[0], pos[1], pos[2], pos[3]);
	end
	
	if (actor == nil) then
		player:SendMessage(0x20, "", "This actor class id cannot be spawned.");
	end
	
end;