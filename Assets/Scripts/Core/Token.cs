namespace GuaLanguage
{
    public class Token 
    {
        public static readonly Token EOF = new Token(-1, -1, -1); // end of file
        public static readonly string EOL = "\\n";          // end of line 
        private int lineNumber;

        private int st, ed;

        protected Token(int line, int st, int ed) {
            lineNumber = line;
            this.st = st;
            this.ed = ed;
        }
        public virtual int getLineNumber() { return lineNumber; }
        public virtual int getST() { return st; }
        public virtual int getED() { return ed; }
        public virtual bool isIdentifier() { return false; }
        public virtual bool isNumber() { return false; }
        public virtual bool isString() { return false; }
        public virtual int getNumber() { throw new GuaException("not number token"); }
        public virtual string getText() { return ""; }

        public override string ToString()
        {
            return getText();
        }
    }
}
