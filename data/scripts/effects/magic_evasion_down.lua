--Bloodletter2 is the uncomboed version of Bloodletter. It doesn't deal any additional damage when it falls off but has the same tick damage
function onGain(owner, effect)
    owner.SubtractMod(modifiersGlobal.MagicEvasion, effect.GetMagnitude());
end

function onLose(owner, effect)
    owner.AddMod(modifiersGlobal.MagicEvasion, effect.GetMagnitude());
end
