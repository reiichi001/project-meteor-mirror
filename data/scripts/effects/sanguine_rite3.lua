require("modifiers")

function onGain(owner, effect, actionContainer)
    --Traited Sanguine Rite reduces damage taken by 25%.
    --The icon in game says it's 50%, but it's lying
    local amount = 25;

    owner.AddMod(modifiersGlobal.DamageTakenDown, amount);
end;

function onLose(owner, effect, actionContainer)    
    local amount = 25;

    owner.SubtractMod(modifiersGlobal.DamageTakenDown, amount);
end;

--Sanguine Rite restores 30% of damage taken as MP
function onDamageTaken(effect, attacker, defender, skill, action, actionContainer)
    local mpToRestore = action.amount * 0.30;
    defender.AddMP(mpToRestore);
    actionContainer.AddMPAction(defender.actorId, 33011, mpToRestore);
end