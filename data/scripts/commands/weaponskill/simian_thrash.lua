require("global");

function onSkillPrepare(caster, target, skill)
    return 0;
end;

function onSkillStart(caster, target, skill)
    return 0;
end;

function onSkillFinish(caster, target, skill, action)
    local damage = math.random(0, 0);
    
    -- todo: populate a global script with statuses and modifiers
    action.worldMasterTextId = 0x765D;
    
    -- todo: populate a global script with statuses and modifiers
    -- magic.HandleAttackMagic(caster, target, spell, action)
    -- action.effectId = bit32.bxor(0x8000000, spell.effectAnimation, 15636);
    --action.effectId = bit32.bxor(0x8000000, spell.effectAnimation, 15636);
    
    if target.hateContainer then
        target.hateContainer.UpdateHate(caster, damage);
    end;
    return damage;
end;