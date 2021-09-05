namespace GuaLanguage
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using GuaLanguage.AST;
    using static GuaLanguage.Parser;
    public class BasicParser 
    {
        protected HashSet<string> reserved = new HashSet<string>();
        protected Operators operators = new Operators();
        protected Parser expr0;
        protected Parser primary;
        protected Parser factor;
        protected Parser expr;

        protected Parser statement0;
        protected Parser block;
        protected Parser simple;
        protected Parser statement;

        protected Parser program;

        protected Parser repeatEmptyLine; // Add by zzy: 使得块代码的花括号不一定需要紧跟在上一行

        public BasicParser() 
        {
            repeatEmptyLine = rule().SetName("repeatEmptyLine").repeat(rule().sep(Token.EOL));

            expr0 = rule();
            primary = rule(typeof(PrimaryExpr))
            .or(rule().sep("(").ast(expr0).sep(")"),
                rule().number(typeof(NumberLiteral)),
                rule().identifier(typeof(Name), reserved),
                rule().string_(typeof(StringLiteral))
            );
            factor = rule().or(rule(typeof(NegativeExpr)).sep("-").ast(primary), 
                primary);
            expr = expr0.expression(typeof(BinaryExpr), factor, operators);
            statement0 = rule();
            block = rule(typeof(BlockStmnt))
                .sep("{").option(statement0)
                .repeat(rule().sep(";", Token.EOL).option(statement0))
                .sep("}");
            simple = rule(typeof(PrimaryExpr)).ast(expr);
            statement = statement0.or(
                rule(typeof(IfStmnt)).sep("if").ast(expr).option(repeatEmptyLine).ast(block)
                    .option(rule().sep("else").ast(block)),
                rule(typeof(WhileStmnt)).sep("while").ast(expr).option(repeatEmptyLine).ast(block),
                simple
            );
            program = rule().or(statement, rule(typeof(NullStmnt)))
                .sep(";", Token.EOL);

            reserved.Add(";");
            reserved.Add("}");
            reserved.Add(Token.EOL);

            operators.add("=", 1, Operators.RIGHT);
            operators.add("==", 2, Operators.LEFT);
            operators.add(">", 2, Operators.LEFT);
            operators.add("<", 2, Operators.LEFT);
            operators.add("<=", 2, Operators.LEFT);
            operators.add(">=", 2, Operators.LEFT);
            operators.add("+", 3, Operators.LEFT);
            operators.add("-", 3, Operators.LEFT);
            operators.add("*", 4, Operators.LEFT);
            operators.add("/", 4, Operators.LEFT);
            operators.add("%", 4, Operators.LEFT);
        }

        public ASTree parse(Lexer lexer) 
        {
            return program.parse(lexer);
        }
    }
}