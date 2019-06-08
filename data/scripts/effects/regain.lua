require("modifiers")

function onGain(owner, effect, actionContainer)
    owner.AddMod(modifiersGlobal.Regain, effect.GetMagnitude());
end

function onLose(owner, effect, actionContainer)
    owner.SubtractMod(modifiersGlobal.Regain, effect.GetMagnitude());
end
