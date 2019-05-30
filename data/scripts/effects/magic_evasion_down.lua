require("modifiers")

function onGain(owner, effect, actionContainer)
    owner.SubtractMod(modifiersGlobal.MagicEvasion, effect.GetMagnitude());
end

function onLose(owner, effect, actionContainer)
    owner.AddMod(modifiersGlobal.MagicEvasion, effect.GetMagnitude());
end
