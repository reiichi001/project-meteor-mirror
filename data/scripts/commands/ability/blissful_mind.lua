require("global");
require("ability");

function onAbilityPrepare(caster, target, ability)
    return 0;
end;

function onAbilityStart(caster, target, ability)
    --27362: Enhanced Blissful Mind
    if caster.HasTrait(27362) then
        ability.statusTier = 2;
    end
    return 0;
end;

function onSkillFinish(caster, target, skill, action, actionContainer)
    --Blissful Mind
    --223228: Blissful Mind
    --223242: Fully Blissful Mind
    local buff = caster.statusEffects.GetStatusEffectById(223228) or caster.statusEffects.GetStatusEffectById(223242);

    --If we have a buff then Blissful Mind removes that buff and restores MP. Otherwise, it adds the Blissful Mind effect
    if buff ~= nil then
        local amount = buff.GetExtra();
        caster.AddMP(amount);

        actionContainer.AddMPAction(caster.actorId, 33007, amount);
        caster.statusEffects.RemoveStatusEffect(buff, actionContainer, 30329);
    else
        --Blissful mind takes 25% of CURRENT HP and begins storing MP up to that point, at which point the buff changes to indicate its full
        local amount = caster.GetHP() * 0.25;

        caster.DelHP(amount, actionContainer);
        skill.statusMagnitude = amount;

        --DoAction handles rates, buffs, dealing damage
        action.DoAction(caster, target, skill, actionContainer);
    end
end;