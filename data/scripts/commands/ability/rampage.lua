require("global");
require("ability");

function onAbilityPrepare(caster, target, ability)
    return 0;
end;

function onAbilityStart(caster, target, ability)
    --27204: Enhanced Rampage
    if caster.HasTrait(27204) then
        ability.statusTier = 2;
    end
    return 0;
end;

function onSkillFinish(caster, target, skill, action, actionContainer)
    --223207: Berserk
    --223208: Rampage
    --Remove Berserk effect. I'm assuming no message is sent like LNC surges
    caster.statusEffects.RemoveStatusEffect(223207);

    --If caster has rampage already, remove it and send a message.
    local buff = caster.statusEffects.GetStatusEffectById(223208)

    if buff ~= nil then
        caster.statusEffects.RemoveStatusEffect(buff, actionContainer, 30329);
    else
        --DoAction handles rates, buffs, dealing damage
        action.DoAction(caster, target, skill, actionContainer);
    end
end;