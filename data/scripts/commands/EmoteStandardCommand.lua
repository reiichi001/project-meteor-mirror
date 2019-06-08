--[[

EmoteStandardCommand Script

Returns the correct animation and log description when an emote is activated.
If 'motion' parameter is used, it returns the blank description id 10105

--]]

emoteTable = {
	[101] = {animId = 1, descId = 21001}, --Surprised
	[102] = {animId = 2, descId = 21011}, --Angry
	[103] = {animId = 3, descId = 21021}, --Furious
	[104] = {animId = 4, descId = 21031}, --Blush
	[105] = {animId = 5, descId = 21041}, --Bow
	[106] = {animId = 6, descId = 21051}, --Cheer
	[107] = {animId = 7, descId = 21061}, --Clap
	[108] = {animId = 8, descId = 21071}, --Beckon
	[109] = {animId = 9, descId = 21081}, --Comfort
	[110] = {animId = 10, descId = 21091}, --Cry
	[111] = {animId = 11, descId = 21101}, --Dance
	[112] = {animId = 12, descId = 21111}, --Doubt
	[113] = {animId = 13, descId = 21121}, --Doze
	[114] = {animId = 14, descId = 21131}, --Fume
	[115] = {animId = 15, descId = 21141}, --Goodbye
	[116] = {animId = 16, descId = 21151}, --Wave
	[117] = {animId = 17, descId = 21161}, --Huh
	[118] = {animId = 18, descId = 21171}, --Joy
	[119] = {animId = 19, descId = 21181}, --Kneel
	[120] = {animId = 20, descId = 21191}, --Chuckle
	[121] = {animId = 21, descId = 21201}, --Laugh
	[122] = {animId = 22, descId = 21211}, --Lookout
	[123] = {animId = 23, descId = 21221}, --Me 
	[124] = {animId = 24, descId = 21231}, --No
	[125] = {animId = 25, descId = 21241}, --Deny
	[126] = {animId = 26, descId = 21251}, --Panic
	[127] = {animId = 27, descId = 21261}, --Point
	[128] = {animId = 28, descId = 21271}, --Poke
	[129] = {animId = 29, descId = 21281}, --Congratulate
	[130] = {animId = 30, descId = 21291}, --Psych
	[131] = {animId = 31, descId = 21301}, --Salute
	[132] = {animId = 32, descId = 21311}, --Shocked
	[133] = {animId = 33, descId = 21321}, --Shrug
	[134] = {animId = 34, descId = 21331}, --Rally
	[135] = {animId = 35, descId = 21341}, --Soothe
	[136] = {animId = 36, descId = 21351}, --Stagger
	[137] = {animId = 37, descId = 21361}, --Stretch
	[138] = {animId = 38, descId = 21371}, --Sulk
	[139] = {animId = 39, descId = 21381}, --Think
	[140] = {animId = 40, descId = 21391}, --Upset
	[141] = {animId = 41, descId = 21401}, --Welcome
	[142] = {animId = 42, descId = 21411}, --Yes
	[143] = {animId = 43, descId = 21421}, --Thumbs Up
	[144] = {animId = 44, descId = 21423}, --Examine Self
	[145] = {animId = 53, descId = 21425}, --Pose
	[146] = {animId = 50, descId = 21427}, --Storm Salute
	[147] = {animId = 51, descId = 21429}, --Serpent Salute
	[148] = {animId = 52, descId = 21431}, --Flame Salute
	[149] = {animId = 45, descId = 21433}, --Blow Kiss
	[151] = {animId = 47, descId = 21435}, --Grovel
	[152] = {animId = 48, descId = 21437}, --Happy
	[153] = {animId = 49, descId = 21439}, --Disappointed
	[154] = {animId = 46, descId = 10105}, --Air Quotes
	[155] = {animId = 54, descId = 21442}, --Pray
	[156] = {animId = 55, descId = 21445}, --Fire Dance
};


function onEventStarted(player, actor, triggerName, emoteId, showText, arg2, arg3, targetId)

	if (targetId == nil) then
		targetId = 0;
	end
    
	if (player:GetState() == 0 or player:GetState() == 11 or player:GetState() == 13) then	
		emote = emoteTable[emoteId];		
		if (emote ~= nil) then
            if showText == 1 then
                player:doEmote(targetId, emote.animId, emote.descId);
            else
                player:doEmote(targetId, emote.animId, 10105);
            end
		else
			player:SendMessage(0x20, "", string.format("Not implemented; EmoteId: %d", emoteId));
		end
	end
    
	player:EndEvent();
	
end
