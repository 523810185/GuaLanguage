namespace GuaLanguage
{
    using System;
    public class GuaException : Exception
    {
        public GuaException(String m) : base(m) { }
        public GuaException(String m, ASTree t) : base(m + " " + t.location()) { }
    }
}
