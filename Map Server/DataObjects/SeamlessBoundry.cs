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
    class SeamlessBoundry
    {
        public readonly uint regionId;
        public readonly uint zoneId1, zoneId2;
        public readonly float zone1_x1, zone1_y1, zone1_x2, zone1_y2;
        public readonly float zone2_x1, zone2_y1, zone2_x2, zone2_y2;
        public readonly float merge_x1, merge_y1, merge_x2, merge_y2;

        public SeamlessBoundry(uint regionId, uint zoneId1, uint zoneId2, float zone1_x1, float zone1_y1, float zone1_x2, float zone1_y2, float zone2_x1, float zone2_y1, float zone2_x2, float zone2_y2, float merge_x1, float merge_y1, float merge_x2, float merge_y2)
        {
            this.regionId = regionId;
            this.zoneId1 = zoneId1;
            this.zoneId2 = zoneId2;
            this.zone1_x1 = zone1_x1;
            this.zone1_y1= zone1_y1;
            this.zone1_x2 = zone1_x2;
            this.zone1_y2 = zone1_y2;
            this.zone2_x1 = zone2_x1;
            this.zone2_y1 = zone2_y1;
            this.zone2_x2 = zone2_x2;
            this.zone2_y2 = zone2_y2;
            this.merge_x1 = merge_x1;
            this.merge_y1 = merge_y1;
            this.merge_x2 = merge_x2;
            this.merge_y2 = merge_y2;
        }
    }
}
