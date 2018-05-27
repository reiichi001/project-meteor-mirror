require("global");
require("weaponskill");

function onSkillPrepare(caster, target, skill)
    return 0;
end;

function onSkillStart(caster, target, skill)
    return 0;
end;

--Changes status to Bloodletter from Bloodletter2. Changes icon of dot and adds additional damage at the end.
function onCombo(caster, target, skill)
    skill.statusId = 223127;
end;

function onSkillFinish(caster, target, skill, action, actionContainer)
    --calculate ws damage
    action.amount = skill.basePotency;

    --DoAction handles rates, buffs, dealing damage
    action.DoAction(caster, target, skill, actionContainer);
    
    --Try to apply status effect
    action.TryStatus(caster, target, skill, actionContainer, true);
end;