namespace FFXIVClassic_Map_Server.dataobjects
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
