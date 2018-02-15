require("global");
require("weaponskill");

function onSkillPrepare(caster, target, skill)
    return 0;
end;

function onSkillStart(caster, target, skill)
    return 0;
end;

--Reset Berserk effect, increase damage?
function onCombo(caster, target, skill)
    --Get Berserk statuseffect
    local berserk = caster.statusEffects.GetStatusEffectById(223160);

    --if it isn't nil
    if berserk != nil then
        berserk.SetTier(1);
        skill.basePotency = skill.basePotency * 1.5;
    end;

    
end;

function onSkillFinish(caster, target, skill, action)
    return weaponskill.onSkillFinish(caster, target, skill, action);
end;
