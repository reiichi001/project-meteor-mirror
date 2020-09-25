require("modifiers")

function onGain(owner, effect, actionContainer)
    owner.AddMod(modifiersGlobal.DamageTakenDown, 100);
end;

function onLose(owner, effect, actionContainer)
    owner.SubtractMod(modifiersGlobal.DamageTakenDown, 100);
end;