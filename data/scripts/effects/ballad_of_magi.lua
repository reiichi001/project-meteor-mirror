require("modifiers")

function onGain(owner, effect, actionContainer)
    --Only one song per bard can be active, need to figure out a good way to do this
    owner.AddMod(modifiersGlobal.Refresh, effect.GetMagnitude());
end;

function onLose(owner, effect, actionContainer)
    owner.SubtractMod(modifiersGlobal.Refresh, effect.GetMagnitude());
end;