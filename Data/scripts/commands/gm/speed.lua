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
    local s = tonumber(stop) or 0;
    local w = tonumber(walk) or 2;
    local r = tonumber(run) or 5;

    if argc == 1 and tonumber(stop) then
        w = (tonumber(stop) / 2);
        player:ChangeSpeed(0, w, s, s);
        player:SendMessage(MESSAGE_TYPE_SYSTEM_ERROR, "", string.format("[speed] Speed set to 0/%s/%s", w,s));
    elseif argc == 3 then
        player:ChangeSpeed(s, w, r, r);
        player:SendMessage(MESSAGE_TYPE_SYSTEM_ERROR, "", string.format("[speed] Speed set to %s/%s/%s", s,w,r));
    else
        player:ChangeSpeed(0, 2, 5, 5);
        player:SendMessage(MESSAGE_TYPE_SYSTEM_ERROR, "", "[speed] Speed values set to default");
    end
    
end