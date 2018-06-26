require("modifiers")

--100 TP per tick without AF. 133 TP per tick with AF
function onGain(owner, effect)
    owner.AddMod(modifiersGlobal.Regain, effect.GetMagnitude());
end

function onLose(owner, effect)
    owner.SubtractMod(modifiersGlobal.Regain, effect.GetMagnitude());
end
