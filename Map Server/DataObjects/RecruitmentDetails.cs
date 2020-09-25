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

namespace Meteor.Map.dataobjects
{
    class RecruitmentDetails
    {
        public string recruiterName;
        public string comment;

        public uint purposeId;
        public uint locationId;
        public uint subTaskId;
        public uint timeSinceStart;

        public uint[] discipleId = new uint[4];
        public uint[] classjobId = new uint[4];
        public byte[] minLvl = new byte[4];
        public byte[] maxLvl = new byte[4];
        public byte[] num = new byte[4];

    }
}
