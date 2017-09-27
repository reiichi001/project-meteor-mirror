require("global");

properties = {
    permissions = 0,
    parameters = "sss",
    description =
[[
Adds experience <qty> to player or <targetname>.
!giveexp <qty> |
!giveexp <qty> <targetname> |
]],
}

function onTrigger(player, argc, qty, name, lastName)
    local sender = "[giveexp] ";
    
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
        
        player:AddExp(qty, player.charaWork.parameterSave.state_mainSkill[0], 5);
    else
        print(sender.."unable to add experience, ensure player name is valid.");
    end;
end;