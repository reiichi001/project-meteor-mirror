﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFXIVClassic_Map_Server.Actors;
using MoonSharp;
using MoonSharp.Interpreter;

namespace FFXIVClassic_Map_Server.actors.chara.ai
{
    class Action
    {
        public DateTime startTime;
        public uint durationMs;
        public bool checkState;
        // todo: lua function
        Script script;
    }
    class ActionQueue
    {
        private Character owner;
        private Queue<Action> actionQueue;
        private Queue<Action> timerQueue;

        public bool IsEmpty { get { return actionQueue.Count > 0 || timerQueue.Count > 0; } }

        public ActionQueue(Character owner)
        {

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

    }
}
