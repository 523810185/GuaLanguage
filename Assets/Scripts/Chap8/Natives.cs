namespace GuaLanguage 
{
    using System;
    using System.Reflection;
    public class Natives 
    {
        public Environment environment(Environment env) 
        {
            appendNatives(env);
            return env;
        }

        protected void appendNatives(Environment env) 
        {
            append(env, "__log", typeof(UnityEngine.Debug), "Log", new Type[]{typeof(string)});
            append(env, "__logError", typeof(UnityEngine.Debug), "LogError", new Type[]{typeof(string)});
            append(env, "__sqrt", typeof(Math), "Sqrt", new Type[]{typeof(float)});
        }

        protected void append(Environment env, string name, Type type, string methodName, Type[] params_) 
        {
            MethodInfo m = null;
            try 
            {
                m = type.GetMethod(methodName, params_);
            }
            catch (Exception e)
            {

            }

            if(m == null) 
            {
                throw new GuaException("cannot find a native function: " + methodName);
            }

            env.put(name, new NativeFunction(methodName, m));
        }
    }
}