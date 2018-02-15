require("global");
require("weaponskill");

function onSkillPrepare(caster, target, skill)
    return 0;
end;

function onSkillStart(caster, target, skill)
    return 0;
end;

--Dispel
--Does dispel have a text id?
function onCombo(caster, target, skill)
    local effects = target.statusEffects.GetStatusEffectsByFlag(16); --lose on dispel
    if effects != nil then
        target.statusEffects.RemoveStatusEffect(effects[0]);
    end;
end;

function onSkillFinish(caster, target, skill, action)
    return weaponskill.onSkillFinish(caster, target, skill, action);
end;