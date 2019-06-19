require("global");

properties = {
    permissions = 0,
    parameters = "s",
    description = "reloads <zone>",
}

function onTrigger(player, argc, zone)
    if not player and not zone or tonumber(zone) == 0 then
        printf("No valid zone specified!");
        return;
    end;
    
    local sender = "[reloadzones] ";
    
    zone = tonumber(zone);
    
    if player then
        local messageID = MESSAGE_TYPE_SYSTEM_ERROR;
        zone = zone or player:GetZoneID();
        player:SendMessage(messageID, "[reloadzones] ", string.format("Reloading zone: %u", zone));
    --[[ todo: get this working legit
        player:GetZone():Clear();
        player:GetZone():AddActorToZone(player);
        player:SendInstanceUpdate();
        ]]
    end;
    
    GetWorldManager():ReloadZone(zone);
    printf("%s reloaded zone %u", sender, zone);
end;