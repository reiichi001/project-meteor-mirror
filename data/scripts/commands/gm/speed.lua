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

 
 if argc == 1 then
 	s = 0;
 	w = (tonumber(stop) / 2);
 	r = tonumber(stop);
 	player:ChangeSpeed(s, w, r);
 	player:SendMessage(MESSAGE_TYPE_SYSTEM_ERROR, "[speed]", string.format("Speed set to 0/%u/%u", w,r));
 
 elseif argc == 3 then
    stop = tonumber(stop) or 0;
    walk = tonumber(walk) or 2;
    run = tonumber(run) or 5;
    player:ChangeSpeed(stop, walk, run);
    player:SendMessage(MESSAGE_TYPE_SYSTEM_ERROR, "[speed]",  string.format("Speed set to %u/%u/%u", stop, walk, run));
    else
    	player:ChangeSpeed(0.0, 2.0, 5.0);
 end   
    
end;