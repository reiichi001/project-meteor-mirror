require("global");

properties = {
    permissions = 0,
    parameters = "ssss",
    description = "adds <item> <qty> to <location> for <target>. <qty> and <location> are optional, item is added to user if <target> is nil",
}

function onTrigger(player, argc, item, qty, location, target)
    local sender = "[giveitem] ";
    player = GetWorldManager():GetPCInWorld(target) or player;
    if player then
        item = tonumber(item) or nil;
        qty = tonumber(qty) or 1;
        location = tonumber(itemtype) or INVENTORY_NORMAL;
        
        if item then
            player:GetInventory(location):AddItem(item, qty);
            player:SendMessage(MSG_TYPE_SYSTEM_ERROR, "[giveitem] ", string.format("Added item %u to %s", item, player:GetName());
        end
    end;
end;