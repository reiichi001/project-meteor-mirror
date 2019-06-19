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

using System;

namespace Meteor.Lobby.DataObjects
{ 
    class Character
    {
        public uint id;
        public ushort slot;
        public ushort serverId;
        public string name;
        public ushort state;
        public string charaInfo;
        public bool isLegacy;
        public bool doRename;
        public uint currentZoneId;

        public byte guardian;
        public byte birthMonth;
        public byte birthDay;

        public uint currentClass = 3;
        public uint currentJob = 0;
        public int currentLevel = 1;

        public byte initialTown;
        public byte tribe;

        public static CharaInfo EncodedToCharacter(String charaInfo)
        {
            charaInfo.Replace("+", "-");
            charaInfo.Replace("/", "_");
            byte[] data = System.Convert.FromBase64String(charaInfo);            

            Program.Log.Debug("------------Base64 printout------------------");
            Program.Log.Debug(Common.Utils.ByteArrayToHex(data));
            Program.Log.Debug("------------Base64 printout------------------");

            CharaInfo chara = new CharaInfo();

            return chara;
        }
    }
}
