require("global");

properties = {
    permissions = 0,
    parameters = "ssss",
    description =
[[
Add/Remove Quests, modify <phase> and <flag 0-32>.
!quest add/remove <quest> |
!quest phase <quest> <phase> |
!quest flag <quest> <flag> true/false |
]],
}

function onTrigger(player, argc, command, var1, var2, var3)

    local messageID = MESSAGE_TYPE_SYSTEM_ERROR;
    local sender = "[quest] ";
    local message = "Error";
    
    if player then
        if argc == 2 then
            if command == "add" or command == "give" or command == "+" then
                if tonumber(var1) then	
                    if player:HasQuest(tonumber(var1)) == false then
                        player:AddQuest(tonumber(var1));
                        message = ("adding quest "..var1);
                    else
                        message = ("already have quest "..var1);
                    end
                else
                    if player:HasQuest(var1) == false then
                        player:AddQuest(var1);
                        message = ("adding quest "..var1);
                    else 
                        message = ("already have quest "..var1);
                    end
                end
                
            elseif command == "remove" or command == "delete" or command == "-" then
                if tonumber(var1) and player:HasQuest(tonumber(var1)) == true then
                    player:RemoveQuestByQuestId(tonumber(var1));
                    message = ("removing quest "..var1);
                else
                    if player:HasQuest(var1) == true then  
                       q2 = GetStaticActor(var1);
                       
                        if q2 ~= nil then 
                        q3 = q2.actorId;
                            message = ("removing quest "..var1);
                            printf(q3);
                            q4 = bit32.band(q3, 0xA0F00000);
                            printf(q4);
                            
                            --player:RemoveQuest(quest.actorName);
                        end
                    else
                        message = ("remove error: either incorrect ID or quest "..var1.." isn't active on character");
                    end
                end
            else
                message = ("error: command "..command.." not recognized");
            end
                
        elseif argc == 3 then
            if command == "phase" or command == "step" then
                if (tonumber(var1) and tonumber(var2)) ~= nil then
                    if player:HasQuest(tonumber(var1)) == true then
                        player:GetQuest(tonumber(var1)):NextPhase(tonumber(var2));
                        message = ("changing phase of quest "..var1.." to "..var2);
                    else
                        message = ("phase error: either incorrect ID or quest "..var1.." isn't active on character");
                    end
                else
                    message = ("error: invalid parameters used");
                end
            else
                message = ("error: command "..command.." not recognized");
            end;
        
        elseif argc == 4 then
            if command == "flag" then
                if tonumber(var1) and (tonumber(var2) >= 0 and tonumber(var2) <= 32) then
                    questvar = tonumber(var1);
                    flagvar = (tonumber(var2));
                    boolvar = 0;              
                    
                    if var3 == "true" or var3 == "1" or var3 == "on" then
                        boolvar = true;                       
                    elseif var3 == "false" or var3 == "0" or var3 == "off" then
                        boolvar = false;
                    elseif var3 == "flip" or var3 == "toggle" then
                        if player:HasQuest(questvar) == true then
                            boolvar = not player:GetQuest(questvar):GetQuestFlag(flagvar);
                        end
                    else
                        message = ("error: flag: boolean not recognized");
                        print(sender..message);
                        return;
                    end
                    
                    var4 =  player:GetQuest(questvar):GetQuestFlag(flagvar);
                    
                    if var4 ~= boolvar then
                        player:GetQuest(questvar):SetQuestFlag(flagvar, boolvar);
                        player:GetQuest(questvar):SaveData();	
                        if boolvar == true then
                            message = ("changing flag "..tonumber(var2).." to true on quest "..questvar);
                        else
                            message = ("changing flag "..tonumber(var2).." to false on quest "..questvar);
                        end
                    else
                        message = ("error: flag "..flagvar.." is already set to that state on quest "..questvar);
                    end    
                else
                    message = ("error: command "..command.." not recognized");
                end
            end	
        end
    end
    
    player:SendMessage(messageID, sender, message);
    print(sender..message);
end