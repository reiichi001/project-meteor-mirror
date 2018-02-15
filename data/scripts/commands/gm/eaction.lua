require("global");

properties = {
    permissions = 0,
    parameters = "s",
    description =
[[
Equips <commandid> in the first open slot without checking if you can.
!eaction <commandid>
]],
}

function onTrigger(player, argc, commandid)
    local sender = "[eaction] ";
    
    print(commandid);
    if name then
        if lastName then
            player = GetWorldManager():GetPCInWorld(name.." "..lastName) or nil;
        else
            player = GetWorldManager():GetPCInWorld(name) or nil;
        end;
    end;
    
    if player then
        classid = player:GetCurrentClassOrJob();
        commandid = tonumber(commandid) or 0;

        local added = player:EquipAbilityInFirstOpenSlot(classid, commandid);
        
    else
        print(sender.."unable to add command, ensure player name is valid.");
    end;
end;