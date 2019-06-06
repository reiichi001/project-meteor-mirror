using System;
using System.Collections.Generic;
using FFXIVClassic_Map_Server.Actors;
using FFXIVClassic_Map_Server.lua;

namespace FFXIVClassic_Map_Server.actors.chara.ai
{
    class Action
    {
        public DateTime startTime;
        public uint durationMs;
        public bool checkState;
        // todo: lua function
        LuaScript script;
    }

    class ActionQueue
    {
        private Character owner;
        private Queue<Action> actionQueue;
        private Queue<Action> timerQueue;

        public bool IsEmpty { get { return actionQueue.Count > 0 || timerQueue.Count > 0; } }

        public ActionQueue(Character owner)
        {
            this.owner = owner;
            actionQueue = new Queue<Action>();
            timerQueue = new Queue<Action>();
        }

        public void PushAction(Action action)
        {

        }

        public void Update(DateTime tick)
        {

        }

        public void HandleAction(Action action)
        {

        }

        public void CheckAction(DateTime tick)
        {

        }
    }
}
