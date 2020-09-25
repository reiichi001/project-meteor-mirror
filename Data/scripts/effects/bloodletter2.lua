require("modifiers")

--Bloodletter2 is the uncomboed version of Bloodletter. It doesn't deal any additional damage when it falls off but has the same tick damage
function onGain(owner, effect, actionContainer)
    owner.AddMod(modifiersGlobal.RegenDown, 15);
end

function onLose(owner, effect, actionContainer)
    owner.SubtractMod(modifiersGlobal.RegenDown, 15);
end
