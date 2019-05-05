require("global");
require("bit32");

properties = {
    permissions = 0,
    parameters = "iiii",
    description = 
[[
effect
]],
}

function onTrigger(player, argc, effectId, magnitude, tick, duration)
    local messageId = MESSAGE_TYPE_SYSTEM_ERROR;
    local sender = "effect";
    
    if player then
        player.AddHP(100000);
        player.DelHP(500);
        
        effectId = tonumber(effectId) or 223180;
        magnitude = tonumber(magnitude) or 700;
        tick = tonumber(tick) or 3;
        duration = tonumber(duration) or 360;
        
        while player.statusEffects.HasStatusEffect(effectId) do
            player.statusEffects.RemoveStatusEffect(effectId);
        end;
        player.statusEffects.AddStatusEffect(effectId, magnitude, tick, duration);
    end;
end;