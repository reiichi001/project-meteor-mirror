require("global");
require("weaponskill");

function onSkillPrepare(caster, target, skill)
    return 0;
end;

function onSkillStart(caster, target, skill)
    return 0;
end;
    
function onSkillFinish(caster, target, skill, action)
    --chance to influct stun only when target has no enmity towards you
    if !(target.hateContainer.HasHateForTarget(caster)) then
        skill.statusChance = 0.50;
    end
    return weaponskill.onSkillFinish(caster, target, skill, action);
end;
