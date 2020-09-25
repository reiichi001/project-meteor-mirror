require("global");
require("ability");

function onAbilityPrepare(caster, target, ability)
    return 0;
end;

function onAbilityStart(caster, target, ability)
    return 0;
end;

function onSkillFinish(caster, target, skill, action, actionContainer)
    --QS gives 300 TP by default.
    skill.statusMagnitude = 300;

    --27241: Enhanced Quelling Strike: Increases TP gained from QS by 50%
    if caster.HasTrait(27241) then
        skill.statusMagnitude = skill.statusMagnitude * 1.5;
    end

    --When raging strikes is active, Quelling Strikes removes it and immediately restores 100 TP for each tier ofr Raging Strikes.
    local buff = caster.statusEffects.GetStatusEffectById(223221)

    if buff ~= nil then
        skill.tpCost = -100 * (buff.GetTier() - 1);
        --QS doesn't send a message
        caster.statusEffects.RemoveStatusEffect(buff);
    end

    --DoAction handles rates, buffs, dealing damage
    action.DoAction(caster, target, skill, actionContainer);
end;