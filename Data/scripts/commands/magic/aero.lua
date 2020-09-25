require("global");
require("magic");

function onMagicPrepare(caster, target, spell)
    return 0;
end;

function onMagicStart(caster, target, spell)
    return 0;
end;

function onSkillFinish(caster, target, skill, action, actionContainer)
    --calculate damage
    action.amount = skill.basePotency;
    skill.statusMagnitude = 15;
    
    --DoAction handles rates, buffs, dealing damage
    action.DoAction(caster, target, skill, actionContainer);

    --Try to apply status effect
    action.TryStatus(caster, target, skill, actionContainer, true);
end;