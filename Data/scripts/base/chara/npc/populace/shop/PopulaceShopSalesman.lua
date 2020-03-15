--[[

PopulaceShopSalesman Script

Functions:

welcomeTalk(sheetId, player) - Start Message
selectMode(askMode) - Shows buy/sell modes. If askmode > 0 show guild tutorial. If askmode == -7/-8/-9 show nothing. Else show affinity/condition tutorials.
selectModeOfClassVendor() - Opens categories for class weapons and gear
selectModeOfMultiWeaponVendor(consumptionmenuId) - Opens categories for weapons/tools (war/magic/land/hand). Arg consumptionmenuId appends location of item repair person. -1: Ul'dah, -2: Gridania, -3: Limsa 
selectModeOfMultiArmorVendor(consumptionmenuId) - Opens categories for armor in different slots. Arg consumptionmenuId appends location of item repair person. -1: Ul'dah, -2: Gridania, -3: Limsa 

openShopBuy(player, shopPack, CurrencyItemId) - ShopPack: Items to appear in window. CurrencyItemId: What is being used to buy these items.
selectShopBuy(player) - Call after openShopBuy() to open widget
closeShopBuy(player) - Closes the buy window

openShopSell(player) - Call this to open sell window
selectShopSell(player) - Call after openShopSell()
closeShopSell(player) - Closes the sell window

confirmSellingItem(itemId, quality, quantity, gil) - Simple Sell confirmation window

selectFacility(?, sheetId, 3) - Opens the facility chooser.
confirmUseFacility(player, cost) - Facility cost confirm

informSellPrice(1, chosenItem, price) - Shows sell confirm window. ChosenItem must be correct.

startTutorial(nil, menuId) - Opens up a tutorial menu for each guild type based on menuId

finishTalkTurn() - Done at the end.

--]]

require ("global")
require ("shop")

shopInfo = { 
--[[ 
    [actorclass id] = 
        { 
            welcomeText   - Dialog for the NPC to speak when interacting
            menuId,       - Displays certain menu/dialog.  29-36 = DoH Facilities menus.  -1 Ul'dah, -2 Gridania, -3 Limsa.  -7/-8/-9/nil show nothing
            shopMode,     - Type of shop. 0 = Single shop pack, 1 = Class vendor, 2 = Weapon vendor, 3 = Armor vendor, 4 = Hamlet vendor
            shopPack{s},  - The item table index to send the client containing the list of items to display, shopmode 2/3 have a static list
        }
--]]
[1000159] = {34, 36, 0, 1016},
[1000163] = {49, 31, 0, 1017},
[1000165] = {74, -8, 0, 1019},
[1001458] = {44, 30, 0, 1018},
[1500142] = {266, -1, 0, 5001},
[1500143] = {267, -1, 0, 5002},
[1500144] = {268, -1, 0, 5003},
[1500145] = {269, -1, 0, 5004},
[1500146] = {269, -1, 0, 5005},
[1500147] = {270, -1, 0, 5006},
[1500150] = {266, -8, 0, 5001},
[1500151] = {267, -8, 0, 5002},
[1500152] = {268, -8, 0, 5003},
[1500153] = {269, -8, 0, 5004},
[1500154] = {269, -8, 0, 5005},
[1500155] = {270, -8, 0, 5006},
[1500158] = {266, -8, 0, 5001},
[1500159] = {267, -8, 0, 5002},
[1500160] = {268, -8, 0, 5003},
[1500161] = {269, -8, 0, 5004},
[1500162] = {269, -8, 0, 5005},
[1500163] = {270, -8, 0, 5006},
[1500401] = {317, -8, 0, 1013},
[1500405] = {320, -8, 0, 1013},
[1500407] = {321, -8, 0, 1012},
[1500411] = {322, -8, 0, 2017},
[1500414] = {324, -8, 0, 1012},
[1500419] = {327, -8, 0, 1012},
[1500422] = {332, -8, 0, 1013},
[1500423] = {331, -8, 0, 2017},
[1500429] = {328, -8, 0, 2017},
[1500430] = {281, -8, 4, 5122},
[1500431] = {281, -8, 4, 5118},
[1500432] = {281, -8, 4, 5120},
[1600001] = {6, -8, 0, 1006},
[1600002] = {7, -8, 0, 1007},
[1600003] = {8, -8, 0, 1008},
[1600004] = {9, -8, 0, 1009},
[1600005] = {10, -8, 0, 1010},
[1600006] = {11, -8, 0, 1011},
[1600007] = {12, -8, 0, 1012},
[1600008] = {13, -8, 0, 1013},
[1600009] = {14, -8, 0, 1014},
[1600010] = {15, -8, 0, 1015},
[1600011] = {1, -8, 0, 1001},
[1600012] = {2, -8, 0, 1002},
[1600013] = {3, -8, 0, 1003},
[1600014] = {4, -8, 0, 1004},
[1600016] = {5, -8, 0, 1005},
[1600017] = {39, 29, 0, 2020},
[1600018] = {59, 33, 0, 2021},
[1600019] = {75, -8, 0, 2022},
[1600020] = {77, -8, 0, 2010},
[1600021] = {78, -8, 0, 2011},
[1600022] = {79, -8, 0, 2012},
[1600023] = {80, -8, 0, 2013},
[1600024] = {81, -8, 0, 2014},
[1600025] = {82, -8, 0, 2015},
[1600026] = {83, -8, 0, 2016},
[1600027] = {84, -8, 0, 2017},
[1600028] = {85, -8, 0, 2018},
[1600029] = {86, -8, 0, 2019},
[1600030] = {87, -8, 0, 2001},
[1600031] = {88, -8, 0, 2003},
[1600032] = {89, -8, 0, 2002},
[1600033] = {90, -8, 0, 2004},
[1600034] = {91, -8, 0, 2005},
[1600035] = {92, -8, 0, 2006},
[1600036] = {93, -8, 0, 2007},
[1600037] = {94, -8, 0, 2008},
[1600039] = {69, 35, 0, 3020},
[1600040] = {54, 32, 0, 3019},
[1600041] = {64, 34, 0, 3021},
[1600042] = {76, -8, 0, 3022},
[1600043] = {96, -8, 0, 3009},
[1600044] = {97, -8, 0, 3010},
[1600045] = {98, -8, 0, 3011},
[1600046] = {99, -8, 0, 3012},
[1600047] = {100, -8, 0, 3013},
[1600048] = {101, -8, 0, 3014},
[1600049] = {102, -8, 0, 3016},
[1600050] = {103, -8, 0, 3015},
[1600051] = {104, -8, 0, 3017},
[1600052] = {105, -8, 0, 3004},
[1600053] = {106, -8, 0, 3007},
[1600054] = {107, -8, 0, 3018},
[1600055] = {108, -8, 0, 3006},
[1600056] = {109, -8, 0, 3005},
[1600057] = {110, -8, 0, 3002},
[1600058] = {111, -8, 0, 3003},
[1600059] = {112, -8, 0, 3001},
[1600061] = {95, -8, 0, 2009},
[1600062] = {113, -8, 0, 3008},
[1600063] = {114, -8, 0, 4001},
[1600064] = {235, -8, 0, 2023},
[1600065] = {236, -8, 0, 1020},
[1600066] = {237, -8, 0, 3023},
[1600067] = {238, -8, 0, 5007},
[1600068] = {239, -8, 0, 5007},
[1600069] = {240, -1, 0, 5007},
[1600070] = {241, -8, 0, 5008},
[1600071] = {242, -8, 0, 5008},
[1600072] = {243, -8, 0, 5008},
[1600073] = {244, -8, 1, 5009}, 
[1600074] = {245, -8, 1, 5015},
[1600075] = {246, -8, 1, 5021},
[1600076] = {247, -8, 1, 5027},
[1600077] = {248, -8, 1, 5033},
[1600078] = {249, -8, 1, 5039},
[1600079] = {250, -8, 1, 5045},
[1600080] = {251, -8, 1, 5051},
[1600081] = {252, -8, 1, 5057},
[1600082] = {253, -8, 1, 5063},
[1600083] = {254, -8, 1, 5069},
[1600084] = {255, -8, 1, 5075},
[1600085] = {256, -8, 1, 5081},
[1600086] = {257, -8, 1, 5087},
[1600087] = {258, -8, 1, 5093},
[1600088] = {259, -8, 1, 5099},
[1600089] = {260, -8, 1, 5105},
[1600090] = {261, -8, 1, 5111},
[1600092] = {263, -8, 0, 2024},
[1600093] = {264, -8, 0, 1021},
[1600094] = {265, -8, 0, 3024},
[1600095] = {281, -8, 0, 1005},
[1600096] = {281, -8, 0, 2009},
[1600097] = {281, -8, 0, 4001},
[1600098] = {281, -8, 0, 4002},
[1600099] = {281, -8, 0, 2009},
[1600100] = {281, -2, 2, 0},
[1600101] = {281, -8, 0, 2009},
[1600103] = {281, -8, 0, 3008},
[1600104] = {281, -8, 0, 3008},
[1600107] = {281, -8, 3, 0},
[1600108] = {281, -8, 0, 3008},
[1600109] = {281, -3, 2, 0},
[1600110] = {281, -8, 0, 4001},
[1600111] = {281, -8, 0, 2009},
[1600112] = {281, -8, 0, 4002},
[1600113] = {281, -8, 0, 4001},
[1600117] = {281, -8, 0, 2009},
[1600119] = {281, -2, 3, 0},
[1600120] = {281, -8, 0, 3008},
[1600121] = {281, -8, 0, 2009},
[1600122] = {281, -8, 0, 3008},
[1600125] = {281, -8, 0, 1005},
[1600126] = {281, -8, 0, 3008},
[1600129] = {281, -1, 3, 0},
[1600130] = {281, -8, 0, 4001},
[1600133] = {281, -1, 2, 0},
[1600137] = {281, -8, 0, 1005},
[1600142] = {281, -8, 0, 1005},

}


shopRange = {  --shopRangeStart, shopRangeEnd
[101] = {101001, 101010};
[102] = {102001, 102010};
[103] = {103001, 103010};
[104] = {104001, 104010};
[105] = {105001, 105010};
[106] = {106001, 106010};
[107] = {107001, 107010};
[108] = {108001, 108017};
[109] = {109001, 109015};
[110] = {110001, 110018};
[111] = {111001, 111018};
[112] = {112001, 112018};
[113] = {113001, 113019};
[114] = {114001, 114015};
[115] = {115001, 115015};
[116] = {116001, 116010};
[117] = {117001, 117010};
[118] = {118001, 118010};
[120] = {120001, 120012};
[121] = {121001, 121012};
[122] = {122001, 122012};
[123] = {123001, 123012};
[124] = {124001, 124012};
[125] = {125001, 125012};
[126] = {126001, 126012};
[127] = {127001, 127012};
[128] = {128001, 128012};
[129] = {129001, 129016};
[130] = {130001, 130012};
[131] = {131001, 131012};
[132] = {132001, 132012};
[133] = {133001, 133012};
[134] = {134001, 134016};
[135] = {135001, 135012};
[136] = {136001, 136012};
[137] = {137001, 137012};
[138] = {138001, 138012};
[139] = {139001, 139012};
[140] = {140001, 140012};
[141] = {141001, 141012};
[142] = {142001, 142012};
[143] = {143001, 143016};
[144] = {144001, 144018};
[145] = {1071001, 1071002};
[146] = {1072001, 1072006};
[1001] = {1001001, 1001008};
[1002] = {1002001, 1002008};
[1003] = {1003001, 1003007};
[1004] = {1004001, 1004002};
[1005] = {1005001, 1005017};
[1006] = {1006001, 1006006};
[1007] = {1007001, 1007010};
[1008] = {1008001, 1008009};
[1009] = {1009001, 1009012};
[1010] = {1010001, 1010014};
[1011] = {1011001, 1011010};
[1012] = {1012001, 1012007};
[1013] = {1013001, 1013011};
[1014] = {1014001, 1014006};
[1015] = {1015001, 1015007};
[1016] = {1016001, 1016016};
[1017] = {1018001, 1018010};
[1018] = {1017001, 1017013};
[1019] = {1019001, 1019005};
[1020] = {1066001, 1066004};
[1021] = {1069001, 1069005};
[2001] = {1020001, 1020008};
[2002] = {1021001, 1021006};
[2003] = {1022001, 1022007};
[2004] = {1023001, 1023008};
[2005] = {1024001, 1024003};
[2006] = {1025001, 1025008};
[2007] = {1026001, 1026006};
[2008] = {1027001, 1027004};
[2009] = {1028001, 1028016};
[2010] = {1029001, 1029009};
[2011] = {1030001, 1030008};
[2012] = {1031001, 1031010};
[2013] = {1032001, 1032010};
[2014] = {1033001, 1033012};
[2015] = {1034001, 1034015};
[2016] = {1035001, 1035013};
[2017] = {1036001, 1036006};
[2018] = {1037001, 1037006};
[2019] = {1038001, 1038008};
[2020] = {1039001, 1039009};
[2021] = {1040001, 1040010};
[2022] = {1041001, 1041005};
[2023] = {1065001, 1065006};
[2024] = {1068001, 1068006};
[3001] = {1042001, 1042008};
[3002] = {1043001, 1043008};
[3003] = {1044001, 1044008};
[3004] = {1045001, 1045008};
[3005] = {1046001, 1046010};
[3006] = {1047001, 1047008};
[3007] = {1048001, 1048006};
[3008] = {1049001, 1049016};
[3009] = {1050001, 1050013};
[3010] = {1051001, 1051008};
[3011] = {1052001, 1052009};
[3012] = {1053001, 1053010};
[3013] = {1054001, 1054006};
[3014] = {1055001, 1055013};
[3015] = {1056001, 1056005};
[3016] = {1057001, 1057008};
[3017] = {1058001, 1058011};
[3018] = {1059001, 1059007};
[3019] = {1060001, 1060011};
[3020] = {1061001, 1061014};
[3021] = {1062001, 1062016};
[3022] = {1063001, 1063004};
[3023] = {1067001, 1067008};
[3024] = {1070001, 1070004};
[4001] = {1064001, 1064011};
[4002] = {1064001, 1064011};
[5001] = {2001001, 2001018};
[5002] = {2002001, 2002006};
[5003] = {2003001, 2003010};
[5004] = {2004001, 2004009};
[5005] = {2005001, 2005010};
[5006] = {2006001, 2006012};
[5007] = {2007001, 2007010};
[5008] = {2008001, 2008016};
[5009] = {2009001, 2009007};
[5010] = {2009101, 2009104};
[5011] = {2009201, 2009204};
[5012] = {2009301, 2009304};
[5013] = {2009401, 2009404};
[5014] = {2009501, 2009504};
[5015] = {2010001, 2010004};
[5016] = {2010101, 2010104};
[5017] = {2010201, 2010204};
[5018] = {2010301, 2010304};
[5019] = {2010401, 2010404};
[5020] = {2010501, 2010504};
[5021] = {2011001, 2011004};
[5022] = {2011101, 2011104};
[5023] = {2011201, 2011204};
[5024] = {2011301, 2011304};
[5025] = {2011401, 2011404};
[5026] = {2011501, 2011504};
[5027] = {2012001, 2012007};
[5028] = {2012101, 2012104};
[5029] = {2012201, 2012204};
[5030] = {2012301, 2012304};
[5031] = {2012401, 2012404};
[5032] = {2012501, 2012504};
[5033] = {2013001, 2013004};
[5034] = {2013101, 2013104};
[5035] = {2013201, 2013204};
[5036] = {2013301, 2013304};
[5037] = {2013401, 2013404};
[5038] = {2013501, 2013504};
[5039] = {2014001, 2014007};
[5040] = {2014101, 2014104};
[5041] = {2014201, 2014204};
[5042] = {2014301, 2014304};
[5043] = {2014401, 2014404};
[5044] = {2014501, 2014504};
[5045] = {2015001, 2015007};
[5046] = {2015101, 2015104};
[5047] = {2015201, 2015204};
[5048] = {2015301, 2015304};
[5049] = {2015401, 2015404};
[5050] = {2015501, 2015504};
[5051] = {2016001, 2016006};
[5052] = {2016101, 2016104};
[5053] = {2016201, 2016204};
[5054] = {2016301, 2016304};
[5055] = {2016401, 2016404};
[5056] = {2016501, 2016504};
[5057] = {2017001, 2017006};
[5058] = {2017101, 2017104};
[5059] = {2017201, 2017204};
[5060] = {2017301, 2017304};
[5061] = {2017401, 2017404};
[5062] = {2017501, 2017504};
[5063] = {2018001, 2018006};
[5064] = {2018101, 2018104};
[5065] = {2018201, 2018204};
[5066] = {2018301, 2018304};
[5067] = {2018401, 2018404};
[5068] = {2018501, 2018504};
[5069] = {2019001, 2019006};
[5070] = {2019101, 2019104};
[5071] = {2019201, 2019204};
[5072] = {2019301, 2019304};
[5073] = {2019401, 2019404};
[5074] = {2019501, 2019504};
[5075] = {2020001, 2020006};
[5076] = {2020101, 2020104};
[5077] = {2020201, 2020204};
[5078] = {2020301, 2020304};
[5079] = {2020401, 2020404};
[5080] = {2020501, 2020504};
[5081] = {2021001, 2021006};
[5082] = {2021101, 2021104};
[5083] = {2021201, 2021204};
[5084] = {2021301, 2021304};
[5085] = {2021401, 2021404};
[5086] = {2021501, 2021504};
[5087] = {2022001, 2022006};
[5088] = {2022101, 2022104};
[5089] = {2022201, 2022204};
[5090] = {2022301, 2022304};
[5091] = {2022401, 2022404};
[5092] = {2022501, 2022504};
[5093] = {2023001, 2023006};
[5094] = {2023101, 2023104};
[5095] = {2023201, 2023204};
[5096] = {2023301, 2023304};
[5097] = {2023401, 2023404};
[5098] = {2023501, 2023504};
[5099] = {2024001, 2024006};
[5100] = {2024101, 2024104};
[5101] = {2024201, 2024204};
[5102] = {2024301, 2024304};
[5103] = {2024401, 2024404};
[5104] = {2024501, 2024504};
[5105] = {2025001, 2025006};
[5106] = {2025101, 2025104};
[5107] = {2025201, 2025204};
[5108] = {2025301, 2025304};
[5109] = {2025401, 2025404};
[5110] = {2025501, 2025504};
[5111] = {2026001, 2026006};
[5112] = {2026101, 2026104};
[5113] = {2026201, 2026204};
[5114] = {2026301, 2026304};
[5115] = {2026401, 2026404};
[5116] = {2026501, 2026504};
[5117] = {2026601, 2026606};
[5118] = {2026701, 2026708};
[5119] = {2026801, 2026808};
[5120] = {2026901, 2026908};
[5121] = {2027001, 2027008};
[5122] = {2027101, 2027110};
[5123] = {2027201, 2027211};
}


function init(npc)
    return false, false, 0, 0;  
end

function onEventStarted(player, npc, triggerName)

    npcId = npc:GetActorClassId();
    
    if shopInfo[npcId] == nil then
        errorMsg = string.format("This PopulaceShopSalesman actor has no shop set. Actor Class Id: %s", npcId);
        player:SendMessage(MESSAGE_TYPE_SYSTEM_ERROR, "", errorMsg );
        player:EndEvent();
        return;
    end;
    
    local shopCurrency = 1000001;
    local welcomeText = 1;
    local menuId = shopInfo[npcId][2];
    local shopCategory = shopInfo[npcId][3];

    local itemShop = 0;
    local classShop = 1;
    local weaponShop = 2;
    local armorShop = 3;
    local hamletShop = 4;
    
    local weaponShopPack = {5001,5002,5007,5008};
    local armorShopPack = {5004,5005,5006,5003}; 
    
    local menuBuy = 1;
    local menuBuyCount = 1;                             -- For Shops with multiple buying categories
    local menuSell = 2;
    local menuFacility = 3;
    local menuTutorial = 4;
    local menuClose = -3;
    local menuHasFacility = false;
    local menuHasTutorial = false;
    
    local shopPack = shopInfo[npcId][4];                -- Starting value for the shopPack of the current NPC Actor
    local chosenShopPackage = 0;                        -- Var to send to openShopMenu() once desired shopPack is determined
    local choice; 

    callClientFunction(player, "welcomeTalk", shopInfo[npcId][welcomeText], player);
    
    while (true) do
           
        if (shopCategory == itemShop) then         
            choice = callClientFunction(player, "selectMode", menuId);
            menuHasFacility = true;
            menuHasTutorial = true;
        elseif (shopCategory == classShop) then   
            choice = callClientFunction(player, "selectModeOfClassVendor");                  
            menuBuyCount = 6;
            menuSell = 0;
        elseif (shopCategory == weaponShop) then    
            choice = callClientFunction(player, "selectModeOfMultiWeaponVendor", menuId);   
            menuBuyCount = 4;
            menuSell = 0;
        elseif (shopCategory == armorShop) then     
            choice = callClientFunction(player, "selectModeOfMultiArmorVendor", menuId);    
            menuBuyCount = 4;
            menuSell = 0;   
        elseif (shopCategory == hamletShop) then
            choice = callClientFunction(player, "selectMode", menuId);

            local hamletRegion = shopPack; 
            local hamletPackAleport = {5117, 5122, 5123};
            local hamletPackHyrstmill = {5117, 5118, 5119};
            local hamletPackGoldenBazaar = {5117, 5120, 5121};
            local hamletLevel = 3;   -- Defaulting to highest value for now
            
            if hamletRegion == 5122 then -- Aleport 
                -- hamletLevel = GetHamletStatus(idAleport);
                shopPack = hamletPackAleport[hamletLevel] or 5117;
            elseif hamletRegion == 5118 then -- Hyrstmill
                -- hamletLevel = GetHamletStatus(idHyrstmill);
                shopPack = hamletPackHyrstmill[hamletLevel] or 5117;
            elseif hamletRegion == 5120 then -- The Golden Bazaar           
                -- hamletLevel = GetHamletStatus(idGoldenBazaar);
                shopPack = hamletPackGoldenBazaar[hamletLevel] or 5117;
            end
        end

        
        if choice and (choice >= menuBuy and choice <= menuBuyCount) then
                --player:SendMessage(0x20,"", "Menu option: "..choice);
                
                if (shopCategory == weaponShop) then
                    chosenShopPackage = weaponShopPack[choice];
                elseif (shopCategory == armorShop) then
                    chosenShopPackage = armorShopPack[choice];
                else
                    chosenShopPackage = ((shopPack-1) + choice);
                end

                openShopMenu(
                    player, 
                    menuId, 
                    chosenShopPackage, 
                    shopRange[chosenShopPackage][1],
                    shopRange[chosenShopPackage][2], 
                    shopCurrency
                );                
                    
        elseif (choice == menuSell) then
            openSellMenu(player);
            
        elseif (choice == menuFacility) and (menuHasFacility == true) then
            if menuId > 0 then
                local classFacility = (shopInfo[npcId][1] + 1) or 35;
                facilityChoice = callClientFunction(player, "selectFacility", nil, classFacility, 3);
                
                if facilityChoice == 1 then 
                    callClientFunction(player, "confirmUseFacility", player, 200);
                elseif facilityChoice == 2 then 
                    callClientFunction(player, "confirmUseFacility", player, 400);
                elseif facilityChoice == 3 then 
                    callClientFunction(player, "confirmUseFacility", player, 1000);           
                end
            end        
        elseif (choice == menuTutorial) and (menuHasTutorial == true) then
            callClientFunction(player, "startTutorial", nil, menuId);            
        end 
        
        if (choice == menuClose or choice == nil) then
            break;  
        end 
    end
    
    callClientFunction(player, "finishTalkTurn", player);
    player:EndEvent();
    
end
   


function openShopMenu(player, menuId, shopPack, itemRangeStart, itemRangeEnd, shopCurrency)

    callClientFunction(player, "openShopBuy", player, shopPack, shopCurrency);
    
    player:SendMessage(0x20, "", "shopPack: "..shopPack.."   Range: "..itemRangeStart.."-"..itemRangeEnd);

    while (true) do     
        buyResult, quantity = callClientFunction(player, "selectShopBuy", player);
        
        if (buyResult == 0) then
            callClientFunction(player, "closeShopBuy", player);                 
            break;
        else
            if itemRangeStart and itemRangeEnd then
                itemChosen =  (itemRangeStart - 1) + buyResult;
                
                if (((itemRangeStart-1) + itemChosen) < itemRangeStart) or (itemChosen > itemRangeEnd) then
                        player:SendMessage(0x20, "", "[ERROR] Client selected item exceeds the valid range.");
                        callClientFunction(player, "finishTalkTurn", player);
                        player:EndEvent();
                        return;
                else
                    player:SendMessage(0x20, "", "Item chosen: " .. itemChosen .. "  Quantity: ".. quantity);

                    --[[
                        TO-DO:  Request item information from server table and throw result to purchaseItem()

                        requestItem = GetItemShopInfoThing(itemChosen);
                        purchaseItem(player, INVENTORY_NORMAL, requestItem.id, quantity, requestItem.quality, requestItem.price, shopCurrency);
                    --]]
                end
            end
            
        end  
    end
 
end



function openSellMenu(player)
    callClientFunction(player, "openShopSell", player);

    while (true) do     
        sellResult, sellQuantity, sellState, unknown, sellItemSlot = callClientFunction(player, "selectShopSell", player);
            
        if (sellResult == nil) then
            callClientFunction(player, "closeShopSell", player);                    
            break;
        else
            if sellState == 1 then
                itemToSell = player:GetItemPackage(INVENTORY_NORMAL):GetItemAtSlot(sellItemSlot-1);
                gItemSellId = itemToSell.itemId; 
                gItemQuality = itemToSell.quality;
                gItemPrice = GetItemGamedata(gItemSellId);
                gItemPrice = gItemPrice.sellPrice;
                
                
                if gItemQuality == 2 then       -- +1
                    gItemPrice = (math.floor(gItemPrice * 1.10));
                elseif gItemQuality == 3 then   -- +2
                    gItemPrice = (math.floor(gItemPrice * 1.25));
                elseif gItemQuality == 4 then   -- +3
                    gItemPrice = (math.floor(gItemPrice * 1.50));
                end

                callClientFunction(player, "informSellPrice", 1, sellItemSlot, gItemPrice);

            elseif sellState == nil then
                sellItem(player, gItemSellId, sellQuantity, gItemQuality, gItemPrice, sellItemSlot-1, shopCurrency);
            end
        end
    end
end