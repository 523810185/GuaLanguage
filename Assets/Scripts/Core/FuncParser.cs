namespace GuaLanguage
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using GuaLanguage.AST;
    using static GuaLanguage.Parser;
    public class FuncParser : BasicParser 
    {
        protected Parser param;
        protected Parser params_;
        protected Parser paramsList;
        protected Parser def;
        protected Parser args;
        protected Parser postfix;

        public FuncParser() : base()
        {
            param = rule().identifier(reserved);
            params_ = rule(typeof(ParameterList)).ast(param).repeat(rule().sep(",").ast(param));
            paramsList = rule().sep("(").maybe(params_).sep(")");
            def = rule(typeof(DefStmnt)).sep("def").identifier(reserved).ast(paramsList).option(repeatEmptyLine).ast(block);
            args = rule(typeof(Arguments)).ast(expr).repeat(rule().sep(",").ast(expr));
            postfix = rule().sep("(").maybe(args).sep(")");

            reserved.Add(")");

            primary.repeat(postfix);
            simple.option(args);
            program.insertChoice(def);
        }
    }
}