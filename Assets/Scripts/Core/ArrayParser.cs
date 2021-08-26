namespace GuaLanguage
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using GuaLanguage.AST;
    using static GuaLanguage.Parser;
    public class ArrayParser : ClassParser 
    {
        protected Parser elements;

        public ArrayParser() : base()
        {
            elements = rule(typeof(ArrayLiteral)).ast(expr).repeat(rule().sep(",").ast(expr));

            reserved.Add("]");
            
            primary.insertChoice(rule().sep("[").maybe(elements).sep("]"));
            postfix.insertChoice(rule(typeof(ArrayRef)).sep("[").ast(expr).sep("]"));
        }
    }
}