require("modifiers")

function onGain(owner, effect, actionContainer)
    owner.AddMod(modifiersGlobal.Refresh, effect.GetMagnitude());
end

function onLose(owner, effect, actionContainer)
    owner.SubtractMod(modifiersGlobal.Refresh, effect.GetMagnitude());
end
