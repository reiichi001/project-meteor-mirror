require("global");

properties = {
    permissions = 0,
    parameters = "",
    description = "prints your current in-game position (different to map coords)",
}

function onTrigger(player)
    local pos = player:GetPos();
    local x = pos[0];
    local y = pos[1];
    local z = pos[2];
    local rot = pos[3];
    local zone = pos[4];
      
    local messageID = MESSAGE_TYPE_SYSTEM_ERROR;
    local sender = "[mypos] ";
    local message = string.format("X:%.3f Y:%.3f Z:%.3f (Rotation: %.3f) Zone:%d", x, y, z, rot, zone);

    player:SendMessage(messageID, sender, message);
end;