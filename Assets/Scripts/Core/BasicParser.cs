namespace GuaLanguage
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using GuaLanguage.AST;
    using static GuaLanguage.Parser;
    public class BasicParser 
    {
        static HashSet<string> reserved = new HashSet<string>();
        static Operators operators = new Operators();
        static Parser expr0 = rule();
        static Parser primary = rule(typeof(PrimaryExpr))
            .or(rule().sep("(").ast(expr0).sep(")"),
                rule().number(typeof(NumberLiteral)),
                rule().identifier(typeof(Name), reserved),
                rule().string_(typeof(StringLiteral))
            );
        static Parser factor = rule().or(rule(typeof(NegativeExpr)).sep("-").ast(primary), 
                primary);
        static Parser expr = expr0.expression(typeof(BinaryExpr), factor, operators);

        static Parser statement0 = rule();
        static Parser block = rule(typeof(BlockStmnt))
            .sep("{").option(statement0)
            .repeat(rule().sep(";", Token.EOL).option(statement0))
            .sep("}");
        static Parser simple = rule(typeof(PrimaryExpr)).ast(expr);
        static Parser statement = statement0.or(
            rule(typeof(IfStmnt)).sep("if").ast(expr).ast(block)
                .option(rule().sep("else").ast(block)),
            rule(typeof(WhileStmnt)).sep("while").ast(expr).ast(block),
            simple
        );

        static Parser program = rule().or(statement, rule(typeof(NullStmnt)))
            .sep(";", Token.EOL);

        public BasicParser() 
        {
            reserved.Add(";");
            reserved.Add("}");
            reserved.Add(Token.EOL);

            operators.add("=", 1, Operators.RIGHT);
            operators.add("==", 2, Operators.LEFT);
            operators.add(">", 2, Operators.LEFT);
            operators.add("<", 2, Operators.LEFT);
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