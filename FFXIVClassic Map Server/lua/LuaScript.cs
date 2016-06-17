using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoonSharp;
using MoonSharp.Interpreter;
using NLog;
using MoonSharp.Interpreter.Debugging;
using System.IO;

namespace FFXIVClassic_Map_Server.lua
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
