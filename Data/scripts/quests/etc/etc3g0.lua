-- Level requirement is 5 on any class.  Set to 1 for testing
-- TODO:  Reward handling

--Actor Scripts
--unique/fst0Town01a/PopulaceStandard/kinnison
--unique/fst0Town01a/PopulaceStandard/mestonnaux
--unique/fst0Town01a/PopulaceStandard/sybell
--unique/fst0Town01a/PopulaceStandard/khuma_moshroca
--unique/fst0Town01a/PopulaceStandard/lefwyne
--unique/fst0Town01a/PopulaceStandard/nellaure


--Quest Flags
FLAG_TALKED_MESTONNAUX = 0;
FLAG_TALKED_SYBELL = 1;
FLAG_TALKED_NELLAURE = 2;
FLAG_TALKED_KHUMA_MOSHROCA = 4;
FLAG_TALKED_LEFWYNE = 8;

function checkNextPhase(player)
    ownedQuest = player:GetQuest("Etc3g0");
    if (
        ownedQuest:GetQuestFlag(FLAG_TALKED_MESTONNAUX) == true and 
        ownedQuest:GetQuestFlag(FLAG_TALKED_SYBELL) == true and 
        ownedQuest:GetQuestFlag(FLAG_TALKED_NELLAURE) == true and 
        ownedQuest:GetQuestFlag(FLAG_TALKED_KHUMA_MOSHROCA) == true and 
        ownedQuest:GetQuestFlag(FLAG_TALKED_LEFWYNE) == true
        ) then
        ownedQuest:NextPhase(243);
    end
end


function canAcceptQuest(player)
	return (player:HasQuest("Etc3g0") == false and player:IsQuestCompleted("Etc3g0") == false and player:GetHighestLevel() >= 1);
end

function isObjectivesComplete(player, quest)
	return (quest:GetPhase() == 243);
end


function onAbandonQuest(player, quest)	
	kinnison = GetWorldManager():GetActorInWorldByUniqueId("kinnison");
	mestonnaux = GetWorldManager():GetActorInWorldByUniqueId("mestonnaux");
	sybell = GetWorldManager():GetActorInWorldByUniqueId("sybell");
	khuma_moshroca = GetWorldManager():GetActorInWorldByUniqueId("khuma_moshroca");
	lefwyne = GetWorldManager():GetActorInWorldByUniqueId("lefwyne");
	nellaure = GetWorldManager():GetActorInWorldByUniqueId("nellaure");	
	
	if (kinnison ~= nil and canAcceptQuest(player)) then
		kinnison:SetQuestGraphic(player, 0x2);		
	end	
	
	if (mestonnaux ~= nil) then mestonnaux:SetQuestGraphic(player, 0x0); end
	if (sybell ~= nil) then sybell:SetQuestGraphic(player, 0x0); end
	if (khuma_moshroca ~= nil) then khuma_moshroca:SetQuestGraphic(player, 0x0); end
	if (lefwyne ~= nil) then lefwyne:SetQuestGraphic(player, 0x0); end
	if (nellaure ~= nil) then nellaure:SetQuestGraphic(player, 0x0); end
	
end
