namespace GuaLanguage
{
    using System.Collections.Generic;
    public abstract class ASTree : IEnumerable<ASTree>
    {
        public abstract ASTree child(int i);
        public abstract int numChildren();
        public abstract IEnumerator<ASTree> children();
        public abstract string location();
        public IEnumerator<ASTree> GetEnumerator() { return children(); }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return GetEnumerator(); }
    }
}
