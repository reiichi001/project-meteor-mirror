require("modifiers")

function onGain(owner, effect, actionContainer)
    --Traited increases speed by 20%. Assuming that means it actually increases speed instead of simply offsetting the negative speed it has by default
    local speedModifier = 0.8;
    if effect.GetTier() == 2 then
        speedModifier = 1.2;
    end

    owner.MultiplyMod(modifiersGlobal.MovementSpeed, speedModifier);
end;

function onLose(owner, effect, actionContainer)    
    local speedModifier = 0.8;
    if effect.GetTier() == 2 then
        speedModifier = 1.2;
    end

    owner.DivideMod(modifiersGlobal.MovementSpeed, speedModifier);
end;