require("global");
require("magic");

function onMagicPrepare(caster, target, spell)
    return 0;
end;

function onMagicStart(caster, target, spell)
    return 0;
end;

--Increased damage and conversion to single target
function onCombo(caster, target, spell)
    spell.aoeType = 0;
    spell.basePotency = spell.basePotency * 1.5;
end;

function onSkillFinish(caster, target, skill, action, actionContainer)
    --calculate damage
    action.amount = skill.basePotency;

    --DoAction handles rates, buffs, dealing damage
    action.DoAction(caster, target, skill, actionContainer);

    --Dispels an effect on each target.
    target.statusEffects.RemoveStatusEffect(GetRandomEffectByFlag(8), actionContainer, 30336);
end;