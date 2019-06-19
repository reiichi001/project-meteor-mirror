require("global");

properties = {
    permissions = 0,
    parameters = "d",
    description = "Spawns a actor",
}

function onTrigger(player, argc, actorName)

	if (actorName ~= nil) then		
		zone = player:GetZone();
		actor = zone:DespawnActor(actorName);
	end
	
end;