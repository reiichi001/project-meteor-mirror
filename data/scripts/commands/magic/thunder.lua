function onMagicPrepare(caster, target, spell)
    return 0;
end;

function onMagicStart(caster, target, spell)
    return 0;
end;

function onMagicFinish(caster, target, spell, action)
    local damage = math.random(10, 100);
    print("fuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuckkk")
    
    action.effectId = bit32.bxor(0x8000000, spell.effectAnimation, 15636);
    if target.hateContainer then
        target.hateContainer.AddBaseHate(caster);
        target.hateContainer.UpdateHate(caster, damage);
    end;
    return damage;
end;