function onCommandStart(effect, owner, skill, actionContainer)
    --27259: Light Shot
    if skill.id == 27259 then
        skill.numHits = effect.GetMagnitude();
    end
end;

function onCommandFinish(effect, owner, skill, actionContainer)
    --27259: Light Shot
    if skill.id == 27259 then
        actionContainer.AddAction(owner.statusEffects.RemoveStatusEffectForBattleAction(effect));
    end
end;