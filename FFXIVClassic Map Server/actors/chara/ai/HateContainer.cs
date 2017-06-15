﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFXIVClassic_Map_Server.Actors;

namespace FFXIVClassic_Map_Server.actors.chara.ai
{
    // todo: actually implement enmity properly
    class HateEntry
    {
        public Character actor;
        public uint cumulativeEnmity;
        public uint volatileEnmity;
        public bool isActive;

        public HateEntry(Character actor, uint cumulativeEnmity = 0, uint volatileEnmity = 0, bool isActive = false)
        {
            this.actor = actor;
            this.cumulativeEnmity = cumulativeEnmity;
            this.volatileEnmity = volatileEnmity;
            this.isActive = isActive;
        }
    }

    class HateContainer
    {
        private Dictionary<Character, HateEntry> hateList;
        private Character owner;

        public HateContainer(Character owner)
        {
            this.owner = owner;
            this.hateList = new Dictionary<Character, HateEntry>();
        }

        public void AddBaseHate(Character target)
        {
            if (!HasHateForTarget(target))
                hateList.Add(target, new HateEntry(target, 0, 0, true));
            else
                Program.Log.Error($"{target.actorName} is already on [{owner.actorId}]{owner.actorName}'s hate list!");
        }

        public void ClearHate(Character target = null)
        {
            if (target != null)
            {
                hateList.Remove(target);
            }
            else
            {
                hateList.Clear();
            }
        }

        private void UpdateHate(HateEntry entry)
        {

        }

        public Dictionary<Character, HateEntry> GetHateList()
        {
            // todo: return unmodifiable collection?
            return hateList;
        }

        public bool HasHateForTarget(Character target)
        {
            return hateList.ContainsKey(target);
        }

        public Character GetMostHatedTarget()
        {
            uint enmity = 0;
            Character target = null;

            foreach(var entry in hateList.Values)
            {
                if (entry.cumulativeEnmity > enmity && entry.isActive)
                {
                    enmity = entry.cumulativeEnmity;
                    target = entry.actor;
                }
            }
            return target;
        }
    }
}
