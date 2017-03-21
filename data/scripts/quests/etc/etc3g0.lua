--Quest Flags
TALKED_1 = 0;
TALKED_2 = 1;
TALKED_3 = 2;
TALKED_4 = 4;
TALKED_5 = 8;

function checkNextPhase(player)
	ownedQuest = player:GetQuest("Etc3g0");
	if (
		ownedQuest:GetQuestFlag(TALKED_1) == false and 
		ownedQuest:GetQuestFlag(TALKED_2) == false and 
		ownedQuest:GetQuestFlag(TALKED_3) == false and 
		ownedQuest:GetQuestFlag(TALKED_4) == false and 
		ownedQuest:GetQuestFlag(TALKED_5) == false
		) then
		ownedQuest:NextPhase(243);
	end
end