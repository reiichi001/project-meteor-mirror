--Doesn't do flat damage. 20 on Lv 50 Truffle Hog, 11 on Coincounter, 7 on nael hard, 19 on 52 fachan
function onGain(target, effect)
    owner.AddMod(modifiersGlobal.RegenDown, effect.GetMagnitude());
end;

function onLose(target, effect)
    owner.AddMod(modifiersGlobal.RegenDown, effect.GetMagnitude());
end;