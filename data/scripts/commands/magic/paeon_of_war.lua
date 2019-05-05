require("global");
require("magic");

function onMagicPrepare(caster, target, skill)
    return 0;
end;

function onMagicStart(caster, target, skill)
    --Restores 50 TP/tick normally. With Choral Tights it's 60 TP. With Battle voice it's 100, 120 with Coral Tights.
    --Battle voice is handled in the Battle Voice script
    --Paeon does not scale with level
    local tpPerTick = 50;

    --8051405: Choral Tights: Enhances Paeon Of War
    if caster.HasItemEquippedInSlot(8051405, 12) then
        tpPerTick = 60;
    end

    --223253: Battle Voice
    --Battle Voice doubles effect of songs
    if caster.statusEffects.HasStatusEffect(223253) then
        tpPerTick = tpPerTick * 2;
        --Set status tier so we can check it later when BV falls off
        skill.statusTier = 2;
    end

    skill.statusMagnitude = tpPerTick;
end;

function onSkillFinish(caster, target, skill, action, actionContainer)
    --223224: Swiftsong
    --223254: Ballad Of Magi
    --223256: Minuet of Rigor
    --If target has one of these effects that was from this caster, remove it
    local oldSong;
    local swiftSong = target.statusEffects.GetStatusEffectById(223224);
    local ballad = target.statusEffects.GetStatusEffectById(223254);
    local minuet = target.statusEffects.GetStatusEffectById(223256);
    if swiftSong and swiftSong.GetSource() == caster then
        oldSong = swiftSong;
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