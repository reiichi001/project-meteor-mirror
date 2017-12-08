require("magic");

function onMagicPrepare(caster, target, spell)
    return 0;
end;

function onMagicStart(caster, target, spell)
    return 0;
end;

function onMagicFinish(caster, target, spell, action)
    local damage = math.random(10, 100);
    
    action.worldMasterTextId = 0x765D;
    
    -- todo: populate a global script with statuses and modifiers
    -- magic.HandleAttackMagic(caster, target, spell, action)
    action.effectId = bit32.bxor(0x8000000, spell.effectAnimation, 15636);

    return damage;
end;