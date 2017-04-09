properties = {
    permissions = 0,
    parameters = "sss",
    description = "<stop> <walk> <run> speed",
}

function onTrigger(player, argc, stop, walk, run)
    stop = tonumber(stop) or 0;
    walk = tonumber(walk) or 2;
    run = tonumber(run) or 5;
    if argc == 3 then
		player:ChangeSpeed(stop, walk, run, run);
	elseif argc == 1 then
		player:ChangeSpeed(0, stop/2, stop, stop);
	else
		player:ChangeSpeed(0,2,5,5);
	end
end