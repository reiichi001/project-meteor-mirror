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