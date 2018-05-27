require("modifiers")

function onGain(target, effect)
    --Traited Sanguine Rite reduces damage taken by 25%.
    --The icon in game says it's 50%, but it's lying
    local amount = 25;

    target.AddMod(modifiersGlobal.DamageTakenDown, amount);
end;

function onLose(target, effect)    
    local amount = 25;

    target.SubtractMod(modifiersGlobal.DamageTakenDown, amount);
end;

--Sanguine Rite restores 30% of damage taken as MP
function onDamageTaken(effect, attacker, defender, action, actionContainer)
    local mpToRestore = action.amount * 0.30;
    defender.AddMP(mpToRestore);
    actionContainer.AddMPAction(defender, 33011, mpToRestore);
end