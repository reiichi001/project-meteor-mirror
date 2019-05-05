require("modifiers")

function onGain(target, effect)
    --Parry is .1% per , Random guess but gonna say it gives 20% worth of parry.
    target.AddMod(modifiersGlobal.Parry, 200);
end;

function onParry(effect, attacker, defender, action, actionContainer)
    --Foresight is lost after parrying
    actionContainer.AddAction(defender.statusEffects.RemoveStatusEffectForBattleAction(effect));
end;

function onLose(target, effect)
    target.SubtractMod(modifiersGlobal.Parry, 200);
end;