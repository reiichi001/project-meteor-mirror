require("global");

properties = {
    permissions = 0,
    parameters = "",
    description = 
[[
Get the amount of actors in this zone.
!zonecount
]]
        
}

function onTrigger(player, argc)
 
	local message = tostring(player.zone.GetAllActors().Count);

	player.SendMessage(0x20, "", message);
end