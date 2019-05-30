require("global");

properties = {
    permissions = 0,
    parameters = "sss",
    description =
[[
Sets player or <targetname>'s maximum tp to <tp> and heals them to full.
!setmaxtp <tp> |
!setmaxtp <tp> <targetname>
]],
}

function onTrigger(player, argc, tp)
    local sender = "[setmaxtp] ";
    
    
    
    if player then
        tp = tonumber(tp) or 0;
        location = INVENTORY_CURRENCY;
        
        player:SetTP(tp);
    else
        print(sender.."unable to add experience, ensure player name is valid.");
    end;
end;