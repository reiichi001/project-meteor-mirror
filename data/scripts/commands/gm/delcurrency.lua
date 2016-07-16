require("global");

properties = {
    permissions = 0,
    parameters = "sssss",
    description = "removes <currency> <qty> from <target>, currency is removed from user if <target> is nil",
}

function onTrigger(player, argc, currency, qty, location, name, lastName)
    local sender = "[delcurrency] ";
    
    if name then
        if lastName then
            player = GetWorldManager():GetPCInWorld(name.." "..lastName) or nil;
        else
            player = GetWorldManager():GetPCInWorld(name) or nil;
        end;
    end;
    
    if player then
        currency = tonumber(currency) or nil;
        qty = 1;
        location = INVENTORY_CURRENCY;
        
        local removed = player:GetInventory(location):removecurrency(currency, qty);
        local messageID = MESSAGE_TYPE_SYSTEM_ERROR;
        local message = "unable to remove currency";
        
        if currency and removed then
            message = string.format("removed currency %u from %s", currency, player:GetName());
        end
        player:SendMessage(messageID, sender, message);
        print(message);
    else
        print(sender.."unable to remove currency, ensure player name is valid.");
    end;
end;