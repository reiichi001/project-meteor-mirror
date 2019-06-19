require("global");

properties = {
    permissions = 0,
    parameters = "sss",
    description =
[[
Adds gil <qty> to player or <targetname>.
!givegil <qty> |
!givegil <qty> <targetname> |
]],
}

function onTrigger(player, argc, qty, name, lastName)
    local sender = "[givegil] ";
    
    if name then
        if lastName then
            player = GetWorldManager():GetPCInWorld(name.." "..lastName) or nil;
        else
            player = GetWorldManager():GetPCInWorld(name) or nil;
        end;
    end;
    
    if player then
        currency = 1000001;
        qty = tonumber(qty) or 1;
        location = INVENTORY_CURRENCY;
        
        local added = player:GetItemPackage(location):AddItem(currency, qty, 1);
        local messageID = MESSAGE_TYPE_SYSTEM_ERROR;
        local message = "unable to add gil";
        
        if currency and added then
            message = string.format("added %u gil to %s", qty, player:GetName());
        end
        player:SendMessage(messageID, sender, message);
        print(message);
    else
        print(sender.."unable to add gil, ensure player name is valid.");
    end;
end;