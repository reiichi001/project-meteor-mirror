require("modifiers")

function onGain(owner, effect)
    owner.AddMod(modifiersGlobal.Regen, effect.GetMagnitude());
end

function onLose(owner, effect)
    owner.SubtractMod(modifiersGlobal.Regen, effect.GetMagnitude());
end
