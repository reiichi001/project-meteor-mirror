properties = {
    permissions = 0,
    parameters = "sss",
    description = "<stop> <walk> <run> speed",
}

function onTrigger(player, argc, stop, walk, run)
    stop = tonumber(stop) or 0;
    walk = tonumber(walk) or 2;
    run = tonumber(run) or 5;
    
    player:ChangeSpeed(stop, walk, run, run);
end;