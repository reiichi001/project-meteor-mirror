require("global");

properties = {
    permissions = 0,
    parameters = "ssss",
    description = 
[[
Removes currency <qty> from player or <targetname>
!delcurrency <item> <qty> |
!delcurrency <item> <qty> <targetname> |
]],
}

function onTrigger(player, argc, currency, qty, name, lastName)
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
        qty = tonumber(qty) or 1;
        
        local removed = player:GetItemPackage(INVENTORY_CURRENCY):RemoveItem(currency, qty);
        local messageID = MESSAGE_TYPE_SYSTEM_ERROR;
        local message = "Attempting to remove currency" -- "unable to remove currency";
        
        if currency and removed then
            message = string.format("removed currency %u from %s", currency, player:GetName());
        end
        player:SendMessage(messageID, sender, message);
        print(message);
    else
        print(sender.."unable to remove currency, ensure player name is valid.");
    end;
end;