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

namespace Meteor.Map.Actors.Chara
{
    class CharaWork
    {
        public uint PROPERTY_NAMEPLATE_VISIBLE = 1;
        public uint PROPERTY_NAMEPLATE_VISIBLE2 = 5;

        public ParameterSave    parameterSave = new ParameterSave();
        public ParameterTemp    parameterTemp = new ParameterTemp();
        public BattleSave       battleSave = new BattleSave();
        public BattleTemp       battleTemp = new BattleTemp();
        public EventSave        eventSave = new EventSave();
        public EventTemp        eventTemp = new EventTemp();

        public bool gameParameter = false;


        public byte[]   property = new byte[32];

        public ushort[] status = new ushort[20];
        public uint[]   statusShownTime = new uint[20];

        public uint[]   command = new uint[64]; //ACTORS
        public byte[]   commandCategory = new byte[64];
        public byte     commandBorder = 0x20;
        public bool[]   commandAcquired = new bool[4096];
        public bool[]   additionalCommandAcquired = new bool[36];

        public uint currentContentGroup;
        public uint depictionJudge = 0xa0f50911;
    }
}
