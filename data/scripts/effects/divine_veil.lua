require("modifiers")
require("hiteffect")

--Increases block rate by 100%
function onGain(owner, effect)
    owner.AddMod(modifiersGlobal.RawBlockRate, 100);
end

function onLose(owner, effect)
    owner.SubtractMod(modifiersGlobal.RawBlockRate, 100);
end

--Applys Divine Regen to party in range when healed by cure or cura
function onBlock(caster, target, effect, skill, action, actionContainer)
    --             cure                  cura
    if (skill.id == 27346 or skill.id == 27347) and (caster != owner) then
        --For each party member in range, add divine regen
        for chara in owner.GetPartyMembersInRange(8) do
            local addAction = chara.statusEffects.AddStatusForBattleAction(223264, 2);
            actionContainer.AddAction(addAction);
        end
    end
end;