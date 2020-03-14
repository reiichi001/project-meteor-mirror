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

namespace Meteor.Lobby.DataObjects
{
    class Appearance
    {
        ////////////
        //Chara Info
        public byte size = 0;
        public byte voice = 0;
        public ushort skinColor = 0;

        public ushort hairStyle = 0;
        public ushort hairColor = 0;
        public ushort hairHighlightColor = 0;
        public ushort hairVariation = 0;
        public ushort eyeColor = 0;
        public byte characteristicsColor = 0;

        public byte faceType = 0;
        public byte faceEyebrows = 0;
        public byte faceEyeShape = 0;
        public byte faceIrisSize = 0;
        public byte faceNose = 0;
        public byte faceMouth = 0;
        public byte faceFeatures = 0;
        public byte characteristics = 0;
        public byte ears = 0;

        public uint mainHand = 0;
        public uint offHand = 0;

        public uint head = 0;
        public uint body = 0;
        public uint legs = 0;
        public uint hands = 0;
        public uint feet = 0;
        public uint waist = 0;
        public uint neck = 0;
        public uint rightEar = 0;
        public uint leftEar = 0;
        public uint rightIndex = 0;
        public uint leftIndex = 0;
        public uint rightFinger = 0;
        public uint leftFinger = 0;
        //Chara Info
        ////////////
    }
}
