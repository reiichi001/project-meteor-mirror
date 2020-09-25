require("modifiers")

function onGain(owner, effect, actionContainer)
    owner.AddMod(modifiersGlobal.Regen, effect.GetMagnitude());
end

function onLose(owner, effect, actionContainer)
    owner.SubtractMod(modifiersGlobal.Regen, effect.GetMagnitude());
end
