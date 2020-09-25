require("modifiers")
require("battleutils")

function onGain(owner, effect, actionContainer)
    --Untraited Sentinel is 30% damage taken down, traited is 50%
    local amount = 30;
    if effect.GetTier() == 2 then
        amount = 50;
    end

    owner.AddMod(modifiersGlobal.DamageTakenDown, amount);
end;

function onLose(owner, effect, actionContainer)    
    local amount = 30;
    if effect.GetTier() == 2 then
        amount = 50;
    end

    owner.SubtractMod(modifiersGlobal.DamageTakenDown, amount);
end;

--Increases action's enmity by 100 for weaponskills
--http://forum.square-enix.com/ffxiv/threads/47393-Tachi-s-Guide-to-Paladin-%28post-1.22b%29
--Sentinel only works on weaponskills. It's possible that was a bug because the description says actions
function onHit(effect, attacker, defender, skill, action, actionContainer)
    if skill.GetCommandType() == CommandType.WeaponSkill then
        action.enmity = action.enmity + 100;
    end
end