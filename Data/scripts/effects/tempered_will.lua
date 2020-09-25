require("modifiers")

function onGain(owner, effect, actionContainer)
    owner.AddMod(modifiersGlobal.KnockbackImmune, 1);
end

function onLose(owner, effect, actionContainer)
    owner.SubtractMod(modifiersGlobal.KnockbackImmune, 1);
end