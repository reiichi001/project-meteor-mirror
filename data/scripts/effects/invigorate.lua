require("modifiers")

--100 TP per tick without AF. 133 TP per tick with AF
function onGain(owner, effect, actionContainer)
    owner.AddMod(modifiersGlobal.Regain, effect.GetMagnitude());
end

function onLose(owner, effect, actionContainer)
    owner.SubtractMod(modifiersGlobal.Regain, effect.GetMagnitude());
end
