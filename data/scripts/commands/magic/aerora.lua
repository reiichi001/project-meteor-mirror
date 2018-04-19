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
    spell.potency = spell.potency * 1.5;
end;

function onSkillFinish(caster, target, skill, action, actionContainer)
    --Dispels an effect on each target.
    local effects = target.statusEffects.GetStatusEffectsByFlag2(16); --lose on dispel
    if effects != nil then
        target.statusEffects.RemoveStatusEffect(effects[0]);
    end;
    
    --calculate damage
    action.amount = skill.basePotency;

    --DoAction handles rates, buffs, dealing damage
    action.DoAction(caster, target, skill, actionContainer);

    --Try to apply status effect
    action.TryStatus(caster, target, skill, actionContainer, true);
end;