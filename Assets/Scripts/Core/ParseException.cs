﻿namespace GuaLanguage
{
    using System;
    using System.IO;
    public class ParseException : Exception
    {
        public ParseException(Token t) : this("", t) {
        }
        public ParseException(String msg, Token t) :base("syntax error around " + location(t) + ". " + msg) {

        }
        private static String location(Token t) {
            if (t == Token.EOF)
                return "the last line";
            else
                return "\"" + t.getText() + "\" at line " + t.getLineNumber();
        }
        public ParseException(IOException e) : base("Parse IOException ", e) {
            
        }
        public ParseException(String msg) :base(msg) {

        }
    }
}
