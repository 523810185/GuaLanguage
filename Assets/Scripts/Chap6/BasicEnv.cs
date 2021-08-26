namespace GuaLanguage
{
    using System.Collections.Generic;
    public class BasicEnv : Environment 
    {
        protected Dictionary<string, object> values;
        public BasicEnv() { values = new Dictionary<string, object>(); }
        public void put(string name, object value) 
        { 
            if(values.ContainsKey(name) == false)
            {
                values.Add(name, value);
            }
            else
            {
                values[name] = value;
            }
        }
        public object get(string name) 
        {
            if(name == null) 
            {
                return null;
            }
            
            object ans;
            values.TryGetValue(name, out ans);
            return ans;
        }

        public void setOuter(Environment e) { throw new GuaException(" not imp "); }

        public void putNew(string name, object value) 
        {
            throw new GuaException(" not imp ");
        }

        public Environment where(string name) 
        {
            throw new GuaException(" not imp ");
        }
    }
}