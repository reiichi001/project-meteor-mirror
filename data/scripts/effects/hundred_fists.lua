require("modifiers")

--Set magnitude to milliseconds that HF will reduce delay by
function onGain(owner, effect, actionContainer)
    owner.SubtractMod(modifiersGlobal.Delay, effect.GetMagnitude());
end;

function onLose(owner, effect, actionContainer)
    owner.AddMod(modifiersGlobal.Delay, effect.GetMagnitude());
end;