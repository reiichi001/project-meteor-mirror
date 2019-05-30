require("modifiers")

function onGain(owner, effect, actionContainer)
    owner.SubtractMod(modifiersGlobal.Defense, effect.GetMagnitude());
end

function onLose(owner, effect, actionContainer)
    owner.AddMod(modifiersGlobal.Defense, effect.GetMagnitude());
end
