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

function onTrigger(player, argc, animType, modelAnim, effectId)
    local sender = "[battleaction] ";

    local actor = GetWorldManager():GetActorInWorld(player.currentTarget) or nil;
    if player and actor then
        aid = tonumber(animType) or 0
        mid = tonumber(modelAnim) or 0
        eid = tonumber(effectId) or 0
        local id = bit32.lshift(aid, 24);
        id = bit32.bor(id, bit32.lshift(mid, 12));
        id = bit32.bor(id, eid)
        print((tonumber(id)))
        player:DoBattleAction(30301, id);
    else
        print(sender.."unable to add experience, ensure player name is valid.");
    end;
end;