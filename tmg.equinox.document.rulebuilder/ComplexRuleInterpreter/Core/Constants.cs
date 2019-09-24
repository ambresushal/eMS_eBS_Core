using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.document.rulebuilder.ComplexRuleInterpreter.Core
{
    public class Constants
    {
        public const char START_ARG = '(';
        public const char END_ARG = ')';
        public const char END_LINE = '\n';
        public const char NEXT_ARG = ',';
        public const char QUOTE = '"';
        public const char START_GROUP = '{';
        public const char END_GROUP = '}';
        public const char END_STATEMENT = ';';
        public const char VAR_PREFIX = '$';

        public const string IF = "IF";
        public const string ELSE = "ELSE";
        public const string ELSE_IF = "ELIF";
        public const string WHILE = "WHILE";
        public const string BREAK = "BREAK";
        public const string CONTINUE = "CONTINUE";
        public const string COMMENT = "//";

        public const string ABS = "ABS";
        public const string APPEND = "APPEND";
        public const string REPLACEALL = "REPLACE";
        public const string CD = "CD";
        public const string CD__ = "CD..";
        public const string DIR = "DIR";
        public const string ENV = "ENV";
        public const string EXP = "EXP";
        public const string FINDFILES = "FINDFILES";
        public const string FINDSTR = "FINDSTR";
        public const string INDEX_OF = "INDEXOF";
        public const string KILL = "KILL";
        public const string PI = "PI";
        public const string POW = "POW";
        public const string PRINT = "PRINT";
        public const string PSINFO = "PSINFO";
        public const string PSTIME = "PSTIME";
        public const string PWD = "PWD";
        public const string RUN = "RUN";
        public const string SETENV = "SETENV";
        public const string SET = "SET";
        public const string SIN = "SIN";
        public const string SIZE = "SIZE";
        public const string SQRT = "SQRT";
        public const string SUBSTR = "SUBSTR";
        public const string TOLOWER = "TOLOWER";
        public const string TOUPPER = "TOUPPER";
        public const string SETSOURCE = "SETSOURCE";
        public const string SETTARGET = "SETTARGET";
        public const string COUNT = "COUNT";
        public const string SETTEXT = "SETTEXT";
        public const string CONTAINS = "CONTAINS";
        public const string TRUE = "TRUE";
        public const string APPENDARRAY = "APPENDARRAY";
        public const string APPENDTEXT = "APPENDTEXT";
        public const string SETARRAY = "SETARRAY";
        public const string TOBOOLEAN = "TOBOOLEAN";
        public const string EQUALS = "EQUALS";
        public const string FILTERLIST = "FILTERLIST";
        public const string UPDATETEXT = "UPDATETEXT";
        public const string INTERSECT = "INTERSECT";
        public const string UNION = "UNION";
        public const string EXCEPT = "EXCEPT";
        public const string MERGE = "MERGE";
        public const string CROSSJOIN = "CROSSJOIN";
        public const string SLICEARRAY = "SLICEARRAY";
        public const string UPDATEARRAY = "UPDATEARRAY";
        public const string GETVAL = "GETVAL";
        public const string GETARRAY = "GETARRAY";
        public const string SETDEFAULTROW = "SETDEFAULTROW";
        public const string CREATETOKEN = "CREATETOKEN";
        public const string MERGEARRAY = "MERGEARRAY";
        public const string CONVERTTOTYPE = "CONVERTTOTYPE";
        public const string COUNTARRAY = "COUNTARRAY";
        public const string SETTOKENVALUE = "SETTOKENVALUE";
        public const string DIVIDE = "DIVIDE";
        public const string NUMBERTOWORDS = "NUMBERTOWORDS";

        public static char[] NEXT_ARG_ARRAY = NEXT_ARG.ToString().ToCharArray();
        public static char[] END_ARG_ARRAY = END_ARG.ToString().ToCharArray();
        public static char[] END_LINE_ARRAY = END_LINE.ToString().ToCharArray();
        public static char[] QUOTE_ARRAY = QUOTE.ToString().ToCharArray();

        public static char[] COMPARE_ARRAY = "<>=)".ToCharArray();
        public static char[] IF_ARG_ARRAY = "&|)".ToCharArray();
        public static char[] END_PARSE_ARRAY = ";)}\n".ToCharArray();
        public static char[] NEXT_OR_END_ARRAY = { NEXT_ARG, END_ARG, END_STATEMENT };

        public static char[] TOKEN_SEPARATION = ("<>=+-*/&^|!\n\t " + START_ARG + END_ARG +
                             START_GROUP + NEXT_ARG + END_STATEMENT).ToCharArray();

    }
}
