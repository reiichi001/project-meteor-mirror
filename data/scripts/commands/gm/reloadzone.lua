require("global");

properties = {
    permissions = 0,
    parameters = "s",
    description = "reloads <zone>",
}

function onTrigger(player, argc, zone)
    if not zone or tonumber(zone) == 0 then
        printf("%s is not a valid zone!", zone);
        return;
    end;
    
    zone = tonumber(zone);
    
    if player then
        local messageID = MSG_TYPE_SYSTEM_ERROR;
        player:SendMessage(messageID, "[reloadzones] ", string.format("Reloading zone: %u", zone));
    end;
    
    GetWorldManager():ReloadZone(zone);
end;