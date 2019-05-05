function onGain(owner, effect)
    owner.AddMod(modifiersGlobal.Refresh, effect.GetMagnitude());
end

function onLose(owner, effect)
    owner.SubtractMod(modifiersGlobal.Refresh, effect.GetMagnitude());
end
