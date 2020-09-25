require("global");
require("ability");

function onAbilityPrepare(caster, target, ability)
    return 0;
end;

function onAbilityStart(caster, target, ability)
    --27282: Enhanced Life Surge: Increases effect of Life Surge by 20%
    if caster.HasTrait(27282) then
        ability.statusTier = 2;
    end

    return 0;
end;

function onSkillFinish(caster, target, skill, action, actionContainer)
    --Need a better way to do this
    --223212: Power Surge I
    --223213: Power Surge II
    --223212: Power Surge III
    --No message is sent when PS is removed by Life Surge
    caster.statusEffects.RemoveStatusEffect(223212);
    caster.statusEffects.RemoveStatusEffect(223213);
    caster.statusEffects.RemoveStatusEffect(223214);


    --Using this ability moves to the next LS buff
    local removeId = 0;
    --223215: Life Surge I
    --223216: Life Surge II
    --223217: Life Surge III
    if caster.statusEffects.HasStatusEffect(223215) then
        removeId = 223215;
        skill.statusId = 223216;
        skill.statusTier = 2;
    elseif caster.statusEffects.HasStatusEffect(223216) then
        removeId = 223216;
        skill.statusId = 223217;
        skill.statusTier = 3;
    elseif caster.statusEffects.HasStatusEffect(223217) then
        effect = caster.statusEffects.GetStatusEffectById(223217)
        effect.RefreshTime();
        skill.statusId = 223217;
    end

    if not (removeId == 0) then
        caster.statusEffects.ReplaceEffect(caster.statusEffects.GetStatusEffectById(removeId), skill.statusId, skill.statusTier, skill.statusMagnitude, skill.statusDuration);
    end
    --DoAction handles rates, buffs, dealing damage
    action.DoAction(caster, target, skill, actionContainer);
end;