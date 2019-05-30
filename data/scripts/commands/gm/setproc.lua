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

function onTrigger(player, argc, procid)
    local sender = "[giveexp] ";
    local pid = tonumber(procid)
    player:SetProc(pid, true);
end;