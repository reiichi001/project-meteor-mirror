require("global");

properties = {
    permissions = 0,
    parameters = "ss",
    description =
[[
Sets a modifier of player
!setmod <modId> <modVal> |
]],
}

function onTrigger(player, argc, modId, modVal)
    local sender = "[setmod] ";
    local mod = tonumber(modId)
    local val = tonumber(modVal)
    player:SetMod(mod, val);
end;