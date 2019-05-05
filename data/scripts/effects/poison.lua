function onGain(owner, effect)
    owner.AddMod(modifiersGlobal.RegenDown, effect.GetMagnitude());
end

function onLose(owner, effect)
    owner.SubtractMod(modifiersGlobal.RegenDown, effect.GetMagnitude());
end
