--[[

Guildleve constants and functions

--]]

--Helper functions

function glBorderIconIDToAnimID(iconId)
	return iconId - 20000;
end

function glPlateIconIDToAnimID(iconId)
	return iconId - 20020;
end

function getGLStartAnimationFromSheet(border, plate, isBoost)
	return getGLStartAnimation(glBorderIconIDToAnimID(border), glPlateIconIDToAnimID(plate), isBoost);
end

function getGLStartAnimation(border, plate, isBoost)
	borderBits = border;
	plateBits = bit32.lshift(plate, 7);

	if (isBoost) then
		boostBits = 0x8000; --1 shifted 15 left
	else
		boostBits = 0x0;
	end
	
	return bit32.bor(0x0B000000, boostBits, plateBits, borderBits);
end