require("modifiers")

--will this break with things like slow?
function onGain(target, effect)
    local currDelay = target.GetMod(modifiersGlobal.AttackDelay);
    target.SetMod(modifiersGlobal.AttackDelay), 0.66 * currDelay);
end;

function onLose(target, effect)
    local currDelay = target.GetMod(modifiersGlobal.AttackDelay);
    target.SetMod(modifiersGlobal.AttackDelay), 1.50 * currDelay);
end;

