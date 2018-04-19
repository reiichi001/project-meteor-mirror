require("modifiers")

function onGain(target, effect)
    local speedModifier = 0.5;

    target.SetMod(modifiersGlobal.Speed, target.GetMod(modifiersGlobal.Speed) * speedModifier);
end;

function onLose(target, effect)    
    local speedModifier = 0.5;

    target.SetMod(modifiersGlobal.Speed, target.GetMod(modifiersGlobal.Speed) / speedModifier);
end;