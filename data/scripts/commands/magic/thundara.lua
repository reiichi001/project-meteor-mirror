require("magic");

function onMagicPrepare(caster, target, spell)
    return 0;
end;

function onMagicStart(caster, target, spell)
    return 0;
end;

--Increased Damage and reduced recast time in place of stun
function onCombo(caster, target, spell)
    spell.statusChance = 0;
    spell.basePotency = spell.basePotency * 1.5;
    spell.recastTimeMs = spell.recastTimeMs / 2;
end;

function onSkillFinish(caster, target, skill, action, actionContainer)
    --calculate damage
    action.amount = skill.basePotency;

    --DoAction handles rates, buffs, dealing damage
    action.DoAction(caster, target, skill, actionContainer);

    --Try to apply status effect
    action.TryStatus(caster, target, skill, actionContainer, true);
end;