require("global");
require("weaponskill");

function onSkillPrepare(caster, target, spell)
    return 0;
end;

function onSkillStart(caster, target, spell)
    return 0;
end;

function onSkillFinish(caster, target, spell, action)
    local damage = math.random(10, 100);
    
    -- todo: populate a global script with statuses and modifiers
    action.worldMasterTextId = 0x765D;
    
    -- todo: populate a global script with statuses and modifiers
    -- magic.HandleAttackSkill(caster, target, spell, action)
    -- action.effectId = bit32.bxor(0x8000000, spell.effectAnimation, 15636);
    action.effectId = bit32.bxor(0x8000000, spell.effectAnimation, 15636);
    
    if target.hateContainer then
        target.hateContainer.UpdateHate(caster, damage);
    end;
    return damage;
end;