require("global");
require("ability");

function onAbilityPrepare(caster, target, ability)
    return 0;
end;

function onAbilityStart(caster, target, ability)
    return 0;
end;

function onSkillFinish(caster, target, skill, action, actionContainer)
    --Only the bard gets the Battle Voice effect
    if caster == target then
        local effect = GetWorldManager():GetStatusEffect(223253);
        effect.SetDuration(30);
        caster.statusEffects.AddStatusEffect(effect, caster, actionContainer);
    end

    local effect = GetWorldManager():GetStatusEffect(223029);
    effect.SetDuration(60);
    caster.statusEffects.AddStatusEffect(effect, caster, actionContainer);
end;