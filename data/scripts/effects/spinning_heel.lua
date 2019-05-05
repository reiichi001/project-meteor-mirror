require("modifiers")

function onGain(target, effect)
    target.SetMod(modifiersGlobal.HitCount, 3);
end;

function onLose(target, effect)
    target.SetMod(modifiersGlobal.HitCount, 2);
end;

