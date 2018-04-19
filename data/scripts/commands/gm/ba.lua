require("global");

properties = {
    permissions = 0,
    parameters = "sssss",
    description =
[[
Adds experience <qty> to player or <targetname>.
!giveexp <qty> |
!giveexp <qty> <targetname> |
]],
}

function onTrigger(player, argc, commandId, animationId, textId, effectId, amount)
    local sender = "[battleaction] ";

    if player then
        cid = tonumber(commandId) or 0;
        aid = tonumber(animationId) or 0;
        tid = tonumber(textId) or 0;
        print(effectId)
        eid = tonumber(effectId) or 0;
        amt = tonumber(amount) or 0;
        
        player:DoBattleActionAnimation(cid, aid, tid, eid, amt);
    else
        print(sender.."unable to add experience, ensure player name is valid.");
    end;
end;