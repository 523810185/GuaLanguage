namespace GuaLanguage 
{
    using System.Collections.Generic;
    using GuaLanguage.Utility;

    public class NestedEnv : Environment 
    {
        protected Dictionary<string, object> values;
        protected Environment outer;
        public NestedEnv() :this(null) { }
        public NestedEnv(Environment e) 
        {
            values = new Dictionary<string, object>();
            outer = e;
        }

        public void setOuter(Environment e) { outer = e; }

        public object get(string name) 
        {
            if(name == null) 
            {
                return null;
            }

            object v;
            values.TryGetValue(name, out v);
            if(v == null && outer != null)
            {
                return outer.get(name);
            }
            else 
            {
                return v;
            }
        }
        
        public void putNew(string name, object value) 
        {
            values.TrySet(name, value);
        }

        public void put(string name, object value) 
        {
            var e = where(name);
            if(e == null)
            {
                e = this;
            }
            e.putNew(name, value);
        }

        public Environment where(string name) 
        {
            object _;
            values.TryGetValue(name, out _);
            if(_ != null)
            {
                return this;
            }
            else if(outer == null) 
            {
                return null;
            }
            else 
            {
                return outer.where(name);
            }
        }
    }
}