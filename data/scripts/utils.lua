--[[

Helper Utils

--]]

function getRandomPointInBand(originX, originY, minRadius, maxRadius)
    angle = math.random() * math.pi * 2;
    radius =(math.sqrt(math.random()) * (maxRadius-minRadius)) + minRadius;
    x = radius * math.cos(angle);
    y = radius * math.sin(angle);
    return {x=x+originX,y=y+originY};
end

function getAngleFacing(x, y, targetX, targetY)
	angle = math.atan2(targetX - x, targetY - y);	
	return angle;
end

function getDistanceBetweenActors(actor1, actor2)
	local pos1 = actor1:GetPos();
	local pos2 = actor2:GetPos();

	local dx = pos1[0] - pos2[0];
	local dy = pos1[1] - pos2[1]
	local dz = pos1[2] - pos2[2]
	
	return math.sqrt(dx * dx + dy * dy + dz *dz);
end

function getXZDistanceBetweenActors(actor1, actor2)
	local pos1 = actor1:GetPos();
	local pos2 = actor2:GetPos();

	local dx = pos1[0] - pos2[0];
	local dz = pos1[2] - pos2[2];

	return math.sqrt(dx * dx + dz *dz);
end

function math.Clamp(val, lower, upper)
    if lower > upper then lower, upper = upper, lower end -- swap if boundaries supplied the wrong way
    return math.max(lower, math.min(upper, val))
end