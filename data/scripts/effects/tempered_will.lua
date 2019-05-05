function onGain(owner, effect)
    owner.AddMod(modifiersGlobal.KnockbackImmune, 1);
end

function onLose(owner, effect)
    owner.SubtractMod(modifiersGlobal.KnockbackImmune, 1);
end