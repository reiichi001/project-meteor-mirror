require("modifiers")

function onGain(owner, effect)
    owner.SubtractMod(modifiersGlobal.MagicEvasion, effect.GetMagnitude());
end

function onLose(owner, effect)
    owner.AddMod(modifiersGlobal.MagicEvasion, effect.GetMagnitude());
end
