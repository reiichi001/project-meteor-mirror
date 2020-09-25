/*
===========================================================================
Copyright (C) 2015-2019 Project Meteor Dev Team

This file is part of Project Meteor Server.

Project Meteor Server is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

Project Meteor Server is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with Project Meteor Server. If not, see <https:www.gnu.org/licenses/>.
===========================================================================
*/

namespace Meteor.Map.dataobjects.chara
{
    class PlayerWork
    {
        public byte tribe;
        public byte guardian;
        public byte birthdayMonth;
        public byte birthdayDay;
        public byte initialTown;

        public float restBonusExpRate = 1.5f;

        public uint[] questScenario = new uint[16];
        public uint[] questGuildleve = new uint[8];

        public bool[] questScenarioComplete = new bool[2048];
        public bool[] questGuildleveComplete = new bool[2048];

        public bool isContentsCommand;

        public uint castCommandClient;
        public uint castEndClient;

        public int[] comboNextCommandId = new int[2];
        public float comboCostBonusRate;

        public bool isRemainBonusPoint;

        public bool[] npcLinkshellChatCalling = new bool[64];
        public bool[] npcLinkshellChatExtra = new bool[64];

        public int variableCommandConfirmWarp;
        public string variableCommandConfirmWarpSender;
        public int variableCommandConfirmWarpSenderByID;
        public byte variableCommandConfirmWarpSenderSex;
        public int variableCommandConfirmWarpPlace;

        public int variableCommandConfirmRaise;
        public string variableCommandConfirmRaiseSender;
        public int variableCommandConfirmRaiseSenderByID;
        public byte variableCommandConfirmRaiseSenderSex;
        public int variableCommandConfirmRaisePlace;
                
    }
}
