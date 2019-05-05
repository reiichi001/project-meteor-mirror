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
    --I'm assuming that with raging strikes, that increases to 500.
    --and traited that increases again to 750 (or 450 without RS)
    if caster.statusEffects.HasStatusEffect(223221) then
        actionContainer.AddAction(caster.statusEffects.RemoveStatusEffectForBattleAction(223221));
        skill.statusMagnitude = 500;
    end

    --27241: Enhanced Quelling Strike: Increases TP gained from QS by 50%
    if caster.HasTrait(27241) then
        skill.statusMagnitude = skill.statusMagnitude * 1.5;
    end

    --DoAction handles rates, buffs, dealing damage
    action.DoAction(caster, target, skill, actionContainer);
end;