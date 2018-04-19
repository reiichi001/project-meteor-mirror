require("modifiers");

function onGain(target, effect)
    local currEvade = target.GetMod(modifierGlobals.Evasion);
    target.SetMod(modifierGlobals.Evasion, currEvade + 15);
end;

function onLose(target, effect)
    local currEvade = target.GetMod(modifierGlobals.Evasion);
    target.SetMod(modifierGlobals.Evasion, currEvade - 15);
end;