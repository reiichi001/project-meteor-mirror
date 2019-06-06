require("modifiers")

--Set magnitude to milliseconds that HF will reduce delay by
function onGain(owner, effect, actionContainer)
    owner.MultiplyMod(modifiersGlobal.AttackDelay, effect.GetMagnitude());
end;

function onLose(owner, effect, actionContainer)
    owner.DivideMod(modifiersGlobal.AttackDelay, effect.GetMagnitude());
end;