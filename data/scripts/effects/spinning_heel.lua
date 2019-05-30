require("modifiers")

function onGain(owner, effect, actionContainer)
    owner.SetMod(modifiersGlobal.HitCount, 3);
end;

function onLose(owner, effect, actionContainer)
    owner.SetMod(modifiersGlobal.HitCount, 2);
end;

