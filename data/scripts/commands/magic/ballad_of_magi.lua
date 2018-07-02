require("global");
require("magic");

function onMagicPrepare(caster, target, skill)
    return 0;
end;

function onMagicStart(caster, target, skill)
    --Ballad gives 20 MP a tick at 50
    --BV gives 40 MP per tick
    --Formula seems to be 0.8 * level - 20, not sure why BV gives 71 at 50 then
    local mpPerTick = (0.8 * caster.GetLevel()) - 20;

    --8032705: Choral Shirt: Enhances Ballad of Magi
    --With Choral Shirt, Ballad gives 26 mp a tick. It could be a flat 6 or multiply by 1.3
    --Because minuet seemed like a normal addition I'm assuming this is too
    if caster.HasItemEquippedInSlot(8032705, 10) then
        mpPerTick = mpPerTick + 6;
    end

    --223253: Battle Voice
    --Battle Voice doubles effect of songs
    if caster.statusEffects.HasStatusEffect(223253) then
        mpPerTick = mpPerTick * 2;
        --Set status tier so we can check it later when BV falls off
        skill.statusTier = 2;
    end

    skill.statusMagnitude = mpPerTick;
end;

function onSkillFinish(caster, target, skill, action, actionContainer)
    --223224: Swiftsong
    --223255: Paeon of War
    --223256: Minuet of Rigor
    --
    local oldSong;
    local swiftSong = target.statusEffects.GetStatusEffectById(223224);
    local paeon = target.statusEffects.GetStatusEffectById(223255);
    local minuet = target.statusEffects.GetStatusEffectById(223256);
    if swiftSong and swiftSong.GetSource() == caster then
        oldSong = swiftSong;
    elseif paeon and paeon.GetSource() == caster then
        oldSong = paeon;
    elseif minuet and minuet.GetSource() == caster then
        oldSong = minuet;
    elseif ballad and ballad.GetSource() == caster then
        oldSong = ballad;
    end

    if oldSong then
        target.statusEffects.RemoveStatusEffect(oldSong);
    end
    
    --DoAction handles rates, buffs, dealing damage
    action.DoAction(caster, target, skill, actionContainer);
end;