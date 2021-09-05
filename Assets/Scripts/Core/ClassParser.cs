namespace GuaLanguage
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using GuaLanguage.AST;
    using static GuaLanguage.Parser;
    public class ClassParser : ClosureParser 
    {
        protected Parser member;
        protected Parser class_body;
        protected Parser defclass;

        public ClassParser() : base()
        {
            member = rule().or(def, simple);
            class_body = rule(typeof(ClassBody))
                .option(repeatEmptyLine)
                .sep("{").option(member)
                .repeat(rule().sep(";", Token.EOL).option(member))
                .sep("}");
            defclass = rule(typeof(ClassStmnt)).sep("class").identifier(reserved)
                .option(rule().sep("extends").identifier(reserved))
                .ast(class_body);
            
            postfix.insertChoice(rule(typeof(Dot)).sep(".").identifier(reserved));
            program.insertChoice(defclass);
        }
    }
}