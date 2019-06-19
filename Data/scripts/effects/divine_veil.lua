require("modifiers")

--Increases block rate by 100%
function onGain(owner, effect, actionContainer)
    owner.AddMod(modifiersGlobal.RawBlockRate, 100);
end

function onLose(owner, effect, actionContainer)
    owner.SubtractMod(modifiersGlobal.RawBlockRate, 100);
end

--Applys Divine Regen to party in range when healed by cure or cura
function onHealed(effect, caster, target, skill, action, actionContainer)
    --             cure                  cura
    if (skill.id == 27346 or skill.id == 27347) and (caster != target) then
        local regenDuration = 30;
        --Apparently heals for 85 without AF, 113 with. Unsure if these can be improved with stats
        local magnitude = 85

        --Need a better way to set magnitude when adding effects
        if effect.GetTier() == 2 then
            magnitude = 113;
        end

        --For each party member in range, add divine regen
        for chara in target.GetPartyMembersInRange(8) do
            chara.statusEffects.AddStatusEffect(223264, effect.GetTier(), magnitude, regenDuration, actionContainer);
        end
    end
end;