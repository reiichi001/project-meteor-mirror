require("global");

properties = {
    permissions = 0,
    parameters = "sss",
    description =
[[
Plays animation on target.
!playanimation <animType> <modelAnim> <effectId>
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
        actor:PlayAnimation(id)
    else
        print(sender.."unable to add experience, ensure player name is valid.");
    end;
end;