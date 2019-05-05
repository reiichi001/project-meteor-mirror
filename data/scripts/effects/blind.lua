require("modifiers")

function onGain(owner, effect)
    owner.SubtractMod(modifiersGlobal.Accuracy, effect.GetMagnitude());
end;

function onLose(owner, effect)
    owner.AddMod(modifiersGlobal.Accuracy, effect.GetMagnitude());
end;