require("global");
require("magic");

function onMagicPrepare(caster, target, skill)
    return 0;
end;

function onMagicStart(caster, target, skill)
    return 0;
end;

function onSkillFinish(caster, target, skill, action, actionContainer)
    --223224: Swiftsong
    --223254: Ballad Of Magi
    --223256: Minuet of Rigor
    --If target has one of these effects that was from this caster, remove it
    local oldSong;
    local paeon = target.statusEffects.GetStatusEffectById(223255);
    local ballad = target.statusEffects.GetStatusEffectById(223254);
    local minuet = target.statusEffects.GetStatusEffectById(223256);
    if paeon and paeon.GetSource() == caster then
        oldSong = paeon;
    elseif ballad and ballad.GetSource() == caster then
        oldSong = ballad;
    elseif minuet and minuet.GetSource() == caster then
        oldSong = minuet;
    end

    if oldSong then
        target.statusEffects.RemoveStatusEffect(oldSong);
    end

    --DoAction handles rates, buffs, dealing damage
    action.DoAction(caster, target, skill, actionContainer);
end;