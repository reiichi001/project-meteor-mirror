function onSpellPrepare(caster, target, spell)
    return 0;
end;

function onSpellStart(caster, target, spell)
    return 0;
end;

function onSpellFinish(caster, target, spell, action)
    local damage = math.random(10, 100);
    print("fuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuckkk")
    
    if target.hateContainer then
        target.hateContainer.AddBaseHate(caster);
        target.hateContainer.UpdateHate(caster, damage);
    end;
    return damage;
end;