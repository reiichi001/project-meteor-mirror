require("modifiers")

function onGain(owner, effect)
    --Only one song per bard can be active, need to figure out a good way to do this
    owner.AddMod(modifiersGlobal.Accuracy, effect.GetMagnitude());
    owner.AddMod(modifiersGlobal.MagicAccuracy, effect.GetMagnitude());
end;

function onLose(owner, effect)
    owner.SubtractMod(modifiersGlobal.Accuracy, effect.GetMagnitude());
    owner.SubtractMod(modifiersGlobal.MagicAccuracy, effect.GetMagnitude());
end;