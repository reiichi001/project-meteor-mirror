
function onEventStarted(player, npc)
    defaultWil = GetStaticActor("DftWil");
    player:SendMessage(0x20, "", "This Actorhas no dialog. Actor Class Id: " .. tostring(npc:GetActorClassId()));
   	player:EndEvent();
end

