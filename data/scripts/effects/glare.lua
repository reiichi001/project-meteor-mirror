require("modifiers")

function onGain(owner, effect, actionContainer)
    owner.SubtractMod(modifiersGlobal.Accuracy, effect.GetMagnitude());
end;

function onLose(owner, effect, actionContainer)
    owner.AddMod(modifiersGlobal.Accuracy, effect.GetMagnitude());
end;