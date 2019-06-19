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
using MoonSharp.Interpreter;
using NLog;
using MoonSharp.Interpreter.Debugging;
using System.IO;

namespace Meteor.Map.lua
{
    class LuaScript : Script
    {
        public static Logger Log = LogManager.GetCurrentClassLogger();

        public LuaScript()
        {
            this.Options.DebugPrint = s => { Log.Debug(s); };
        }
        public new static DynValue RunFile(string filename)
        {
            try
            {
                return Script.RunFile(filename);
            }
            catch (Exception e)
            {
                if (e is InterpreterException)
                    Log.Debug(((InterpreterException)e).DecoratedMessage);
                else
                    Log.Debug(e.Message);
                return null;
            }
        }
       
        public new void AttachDebugger(IDebugger debugger)
        {
            try
            {
                ((Script)this).AttachDebugger(debugger);
            }
            catch (Exception e)
            {
                Log.Debug(e.Message);
            }
        }

        public new DynValue Call(DynValue function)
        {
            try
            {
                return ((Script)this).Call(function);
            }
            catch (Exception e)
            {
                if (e is InterpreterException)
                    Log.Debug(((InterpreterException)e).DecoratedMessage);
                else
                    Log.Debug(e.Message);
                return null;
            }
        }

        public new DynValue Call(object function)
        {
            try
            {
                return ((Script)this).Call(function);
            }
            catch (Exception e)
            {
                if (e is InterpreterException)
                    Log.Debug(((InterpreterException)e).DecoratedMessage);
                else
                    Log.Debug(e.Message);
                return null;
            }
        }

        public new DynValue Call(object function, params object[] args)
        {
            try
            {
                return ((Script)this).Call(function, args);
            }
            catch (Exception e)
            {
                if (e is InterpreterException)
                    Log.Debug(((InterpreterException)e).DecoratedMessage);
                else
                    Log.Debug(e.Message);
                return null;
            }
        }

        public new DynValue Call(DynValue function, params DynValue[] args)
        {
            try
            {
                return ((Script)this).Call(function, args);
            }
            catch (Exception e)
            {
                if (e is InterpreterException)
                    Log.Debug(((InterpreterException)e).DecoratedMessage);
                else
                    Log.Debug(e.Message);
                return null;
            }
        }

        public new DynValue Call(DynValue function, params object[] args)
        {
            try
            {
                return ((Script)this).Call(function, args);
            }
            catch (Exception e)
            {
                if (e is InterpreterException)
                    Log.Debug(((InterpreterException)e).DecoratedMessage);
                else
                    Log.Debug(e.Message);
                return null;
            }
        }

        public new DynValue CreateCoroutine(DynValue function)
        {
            try
            {
                return ((Script)this).CreateCoroutine(function);
            }
            catch (Exception e)
            {
                if (e is InterpreterException)
                    Log.Debug(((InterpreterException)e).DecoratedMessage);
                else
                    Log.Debug(e.Message);
                return null;
            }
        }

        public new DynValue CreateCoroutine(object function)
        {
            try
            {
                return ((Script)this).CreateCoroutine(function);
            }
            catch (Exception e)
            {
                if (e is InterpreterException)
                    Log.Debug(((InterpreterException)e).DecoratedMessage);
                else
                    Log.Debug(e.Message);
                return null;
            }
        }

        public new DynValue DoString(string code, Table globalContext = null)
        {
            try
            {
                return ((Script)this).DoString(code, globalContext);
            }
            catch (Exception e)
            {
                if (e is InterpreterException)
                    Log.Debug(((InterpreterException)e).DecoratedMessage);
                else
                    Log.Debug(e.Message);
                return null;
            }
        }

        public new DynValue DoFile(string filename, Table globalContext = null)
        {
            try
            {
                return ((Script)this).DoFile(filename, globalContext);
            }
            catch (Exception e)
            {
                if (e is InterpreterException)
                    Log.Debug(((InterpreterException)e).DecoratedMessage);
                else
                    Log.Debug(e.Message);
                return null;
            }
        }

        public new void Dump(DynValue function, Stream stream)
        {
            try
            {
                ((Script)this).Dump(function, stream);
            }
            catch (Exception e)
            {
                if (e is InterpreterException)
                    Log.Debug(((InterpreterException)e).DecoratedMessage);
                else
                    Log.Debug(e.Message);
            }
        }
        
        public new DynValue RequireModule(string modname, Table globalContext = null)
        {
            try
            {
                return ((Script)this).RequireModule(modname, globalContext);
            }
            catch (Exception e)
            {
                if (e is InterpreterException)
                    Log.Debug(((InterpreterException)e).DecoratedMessage);
                else
                    Log.Debug(e.Message);
                return null;
            }
        }
        
        public new void SetTypeMetatable(DataType type, Table metatable)
        {
            try
            {
                ((Script)this).SetTypeMetatable(type, metatable);
            }
            catch (Exception e)
            {
                if (e is InterpreterException)
                    Log.Debug(((InterpreterException)e).DecoratedMessage);
                else
                    Log.Debug(e.Message);
            }
        }
    }
}
