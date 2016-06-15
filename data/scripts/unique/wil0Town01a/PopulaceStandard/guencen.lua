
function onEventStarted(player, npc)
    defaultWil = getStaticActor("DftWil");
    player:sendMessage(0x20, "", "This Actorhas no dialog. Actor Class Id: " .. tostring(npc:getActorClassId()));
   	player:endEvent();
end

