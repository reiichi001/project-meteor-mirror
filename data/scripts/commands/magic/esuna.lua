require("global");
require("magic");
require("battleutils");

function onMagicPrepare(caster, target, spell)
    if not target.statusEffects.HasStatusEffectsByFlag(StatusEffectFlags.LoseOnDeath) then
        return -1
    end

    return 0;
end;

function onMagicStart(caster, target, spell)
    return 0;
end;

function onSkillFinish(caster, target, skill, action, actionContainer)

    removeEffect = target.statusEffects.GetRandomEffectByFlag(StatusEffectFlags.LoseOnDeath)

    target.statusEffects.RemoveStatusEffect(removeEffect, actionContainer, 30331);
end;