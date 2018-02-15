require("global");

properties = {
    permissions = 0,
    parameters = "sss",
    description = 
[[
Set movement speed for player. Enter no value to reset to default.
!speed <run> | 
!speed <stop> <walk> <run> |
]]
        
}

function onTrigger(player, argc, stop, walk, run)
 
	local message = tostring(player.zone.GetAllActors().Count);

	player.SendMessage(0x20, "", message); 
	
end