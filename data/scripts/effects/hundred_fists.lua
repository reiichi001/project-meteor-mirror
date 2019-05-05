require("modifiers")

--Set magnitude to milliseconds that HF will reduce delay by
function onGain(target, effect)
    target.SubtractMod(modifiersGlobal.AttackDelay), effect.GetMagnitude());
end;

function onLose(target, effect)
    target.AddMod(modifiersGlobal.AttackDelay), effect.GetMagnitude());
end;