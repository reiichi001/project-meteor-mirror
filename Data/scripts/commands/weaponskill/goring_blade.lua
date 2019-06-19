require("global");
require("weaponskill");

function onSkillPrepare(caster, target, skill)
    return 0;
end;

function onSkillStart(caster, target, skill)
    skill.statusMagnitude = 25;--could probalby have a status magnitude value
    return 0;
end;

--Chance to increase defense when executed from behind the target
function onPositional(caster, target, skill)
    skill.statusChance = 0.90;
end;

--Increases bleed damage
--Bleed damage seems like it's 25 with comboed being 38  (25 * 1.5 rounded up)
function onCombo(caster, target, skill)
    skill.statusMagnitude = 38;
end;

function onSkillFinish(caster, target, skill, action, actionContainer)
    --calculate ws damage
    action.amount = skill.basePotency;

    --DoAction handles rates, buffs, dealing damage
    action.DoAction(caster, target, skill, actionContainer);

    --Try to apply status effect
    action.TryStatus(caster, target, skill, actionContainer, true);
end;