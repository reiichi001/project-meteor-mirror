require("global");
require("weaponskill");

function onSkillPrepare(caster, target, spell)
    return 0;
end;

function onSkillStart(caster, target, spell)
    return 0;
end;

--Increased enmity
function onCombo(caster, target, skill)
    --https://www.bluegartr.com/threads/107403-Stats-and-how-they-work/page17
    skill.enmityModifier = 2.75;
end;

function onSkillFinish(caster, target, skill, action, actionContainer)
    --calculate ws damage
    action.amount = skill.basePotency;

    --DoAction handles rates, buffs, dealing damage
    action.DoAction(caster, target, skill, actionContainer);
end;