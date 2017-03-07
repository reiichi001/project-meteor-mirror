require("global");

properties = {
    permissions = 0,
    parameters = "ssss",
    description = "usage: <id> <updateTime> <zonewide>.",
}

function onTrigger(player, argc, weather, updateTime, zonewide)
	-- todo: change weather
    local messageID = MESSAGE_TYPE_SYSTEM_ERROR;
    local sender = "[weather] ";
    local message = "unable to change weather";
    
    if player then
        weather = tonumber(weather) or 0;
        updateTime = tonumber(updateTime) or 0;
        zonewide = tonumber(zonewide) or 0;
        message = "changed weather to %u ";
        if zonewide ~= 0 then
            message = string.format(message.."for zone %u", player:GetZoneID());
        else
            message = message..player:GetName();
        end;
        -- weatherid, updateTime
        player:GetZone():ChangeWeather(weather, updateTime, player, zonewide ~= 0);
        player:SendMessage(messageID, sender, message);
    end;
    print(sender..message);
end;