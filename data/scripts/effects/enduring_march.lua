require("modifiers")

function onGain(target, effect)
    --Traited increases speed by 20%. Assuming that means it actually increases speed instead of simply offsetting the negative speed it has by default
    local speedModifier = 0.8;
    if effect.GetTier() == 2 then
        speedModifier = 1.2;
    end

    target.SetMod(modifiersGlobal.Speed, target.GetMod(modifiersGlobal.Speed) * speedModifier);
end;

function onLose(target, effect)    
    local speedModifier = 0.8;
    if effect.GetTier() == 2 then
        speedModifier = 1.2;
    end

    target.SetMod(modifiersGlobal.Speed, target.GetMod(modifiersGlobal.Speed) / speedModifier);
end;