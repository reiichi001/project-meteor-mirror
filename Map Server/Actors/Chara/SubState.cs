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

namespace Meteor.Map.actors.chara
{
    class SubState
    {
        public byte breakage = 0;
        public byte chantId = 0;
        public byte guard = 0;
        public byte waste = 0;
        public byte mode = 0;
        public ushort motionPack = 0;

        public void toggleBreak(int index, bool toggle)
        {
            if (index > 7 || index < 0)
                return;

            if (toggle)
                breakage = (byte)(breakage | (1 << index)); 
            else
                breakage = (byte)(breakage & ~(1 << index)); 
        }

        public void setChant(byte chant) {
            chantId = chant;
        }

        public void setGuard(byte guard)
        {
            if (guard >= 0 && guard <= 3)
                this.guard = guard;
        }

        public void setWaste(byte waste)
        {
            if (waste >= 0 && waste <= 3)
                this.waste = waste;
        }

        public void setMode(byte bitfield)
        {
            mode = bitfield;
        }

        public void setMotionPack(ushort mp)
        {
            motionPack = mp;
        }

    }
}
