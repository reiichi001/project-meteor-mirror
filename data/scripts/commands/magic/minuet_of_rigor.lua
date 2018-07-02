require("global");
require("magic");

function onMagicPrepare(caster, target, skill)
    return 0;
end;

function onMagicStart(caster, target, skill)
    --Miuet gives 35 ACC/MACC by default at level 50. Minuet does scale with level
    --BV apparetnly gives 71 ACc/MACC
    --Formula seems to be level - 15, not sure why BV gives 71 at 50 then
    local acc = caster.GetLevel() - 15;

    --8071405: Choral Ringbands: Enhances Minuet of Rigor
    --With Choral Tights, Minuet gives 60 ACC/MACC at 50. Unsure what it is at lower levels (ie if it's a flat added 25 MP or a multiplier)
    --Assuming it's a flat 25 because that makes more sense than multiplying by 1.714
    if  caster.HasItemEquippedInSlot(8071405, 13) then
        acc = acc + 25;
    end

    --223253: Battle Voice
    --Battle Voice doubles effect of songs
    if caster.statusEffects.HasStatusEffect(223253) then
        acc = acc * 2;
        --Set status tier so we can check it later when BV falls off
        skill.statusTier = 2;
    end

    skill.statusMagnitude = acc;
end;

function onSkillFinish(caster, target, skill, action, actionContainer)
    --223224: Swiftsong
    --223254: Ballad Of Magi
    --223255: Paeon of War
    --If target has one of these effects that was from this caster, remove it
    local oldSong;
    local swiftSong = target.statusEffects.GetStatusEffectById(223224);
    local ballad = target.statusEffects.GetStatusEffectById(223254);
    local paeon = target.statusEffects.GetStatusEffectById(223255);
    if swiftSong and swiftSong.GetSource() == caster then
        oldSong = swiftSong;
    elseif ballad and ballad.GetSource() == caster then
        oldSong = ballad;
    elseif paeon and paeon.GetSource() == caster then
        oldSong = paeon;
    end

    if oldSong then
        target.statusEffects.RemoveStatusEffect(oldSong);
    end
    
    --DoAction handles rates, buffs, dealing damage
    action.DoAction(caster, target, skill, actionContainer);
end;