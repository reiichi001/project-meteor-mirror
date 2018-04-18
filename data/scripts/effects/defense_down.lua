function onGain(owner, effect)
    owner.SubtractMod(modifiersGlobal.Defense, effect.GetMagnitude());
end

function onLose(owner, effect)
    owner.AddMod(modifiersGlobal.Defense, effect.GetMagnitude());
end
