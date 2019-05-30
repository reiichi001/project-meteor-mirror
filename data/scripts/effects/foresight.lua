require("modifiers")

function onGain(owner, effect, actionContainer)
    --Parry is .1% per , Random guess but gonna say it gives 20% worth of parry.
    owner.AddMod(modifiersGlobal.Parry, 200);
end;

function onParry(effect, attacker, defender, skill, action, actionContainer)
    --Foresight is lost after parrying
    defender.statusEffects.RemoveStatusEffect(effect, actionContainer, 30331, false);
end;

function onLose(owner, effect, actionContainer)
    owner.SubtractMod(modifiersGlobal.Parry, 200);
end;