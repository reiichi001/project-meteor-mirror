require("global");
require("ability");
require("modifiers");

function onAbilityPrepare(caster, target, ability)
    return 0;
end;

function onAbilityStart(caster, target, ability)
    return 0;
end;

function onSkillFinish(caster, target, skill, action, actionContainer)
    --Take off 1/3 of attack delay. Not sure if this is the exact amount HF reduces by
    action.statusMagnitude = 0.33 * caster.GetMod(modifiersGlobal.AttackDelay);
    action.DoAction(caster, target, skill, actionContainer);
end;