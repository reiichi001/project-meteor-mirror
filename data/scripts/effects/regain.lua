function onGain(owner, effect)
    owner.AddMod(modifiersGlobal.Regain, effect.GetMagnitude());
end

function onLose(owner, effect)
    owner.SubtractMod(modifiersGlobal.Regain, effect.GetMagnitude());
end
