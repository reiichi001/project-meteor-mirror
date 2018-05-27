require("global");
require("ability");

function onAbilityPrepare(caster, target, ability)
    return 0;
end;

function onAbilityStart(caster, target, ability)
    return 0;
end;

function onSkillFinish(caster, target, skill, action, actionContainer)
    --For some reason, light shot's hitNum is always 1 (or 0, idk), even with barrage. 
    --If you set the hitnum like any other multi-hit WS it will play the animation repeatedly.
    action.hitNum = 1;

    action.amount = skill.basePotency;
    --DoAction handles rates, buffs, dealing damage
    action.DoAction(caster, target, skill, actionContainer);
end;