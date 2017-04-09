--Quest Flags
TALKED_PFARAHR = 0;

function canAcceptQuest(player)
	return (player:HasQuest("etc5g0") == false and player:IsQuestCompleted("Etc5g0") == false and player:GetHighestLevel() >= 1);
end

function isObjectivesComplete(player, quest)
	return (quest:GetPhase() == 2);
end

function onAbandonQuest(player, quest)	
	vkorolon = GetWorldManager():GetActorInWorldByUniqueId("vkorolon");
	pfarahr = GetWorldManager():GetActorInWorldByUniqueId("pfarahr");
	if (vkorolon ~= nil and canAcceptQuest(player)) then
		vkorolon:SetQuestGraphic(player, 0x2);		
	end	
	if (pfarahr ~= nil) then
		pfarahr:SetQuestGraphic(player, 0x0);
	end
end