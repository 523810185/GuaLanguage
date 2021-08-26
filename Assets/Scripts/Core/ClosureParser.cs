namespace GuaLanguage
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using GuaLanguage.AST;
    using static GuaLanguage.Parser;
    public class ClosureParser : FuncParser 
    {
        public ClosureParser() : base()
        {
            primary.insertChoice(rule(typeof(Fun)).sep("fun").ast(paramsList).ast(block));
        }
    }
}