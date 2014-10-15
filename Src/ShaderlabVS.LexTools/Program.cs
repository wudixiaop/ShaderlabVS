using ShaderlabVS.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaderlabVS.LexTools
{
    class Program
    {
        private static string lexFormat = @"
%namespace ShaderlabVS.Lexer
%option verbose, summary, noparser, nofiles, unicode

/**********************************************************************************/
/********************************User Defined Code*********************************/
/**********************************************************************************/

%{
     public int NextToken() { return yylex(); }
     public int GetPos() { return yypos; }
     public int GetLength() { return yyleng; }
	 public void PushbackText(int length) { yyless(length); }
%}

/********************************Rules Section*********************************/

%x COMMENT
digit               [0-9]
alpha               [a-zA-Z_]
exponent            [Ee](""+""|""-"")?{digit}+
floatsuffix         [fFhH]
hexdigit            [0-9a-fA-F]
float1				(""+""|""-"")?{digit}*"".""{digit}+{floatsuffix}?
float2				(""+""|""-"")?{digit}+"".""{digit}+{floatsuffix}?
numbers				((""+""|""-"")?{digit}+|{float1}|{float2})

symbol				(""+""|""-""|""*""|""/""|""%""|""|""|""<""|"">""|""=""|""!""|"";""|"",""|""=""|""&""|"";""|""{""|""}""|"",""|""(""|"")""|""[""|""]"")

words				[^\*\n]*
blank				"" ""
white_space         [ \t\v\n\f\r]

singleLineComment	""//""[^\n]*
multiLineComment	""/*""[^*]*\*(\*|([^*/]([^*])*\*))*\/

HLSLCGBlockKeyWords	        {$HLSLCGBlockKeyWords$}
HLSLCGNonBlockKeyWords	    {$HLSLCGNonBlockKeyWords$}
HLSLCGSpecialKeyWords       {$HLSLCGSpecialKeyWords$}
HLSLCGDatatypes			    {$HLSLCGDatatypes$}
HLSLCGFunctions		        {$HLSLCGFunctions$}

UNITYBuiltinDataTypes       {$UNITYBuiltinDataTypes$}
UNITYBuiltinFunctions       {$UNITYBuiltinFunctions$}
UNITYBuiltinMacros          {$UNITYBuiltinMacros$}
UNITYBuiltinKeywords        {$UNITYBuiltinKeywords$}
UNITYBuiltinValues          {$UNITYBuiltinValues$}

%%

/*********Comments********/
/*************************/
{singleLineComment}			{return (int)ShaderlabToken.COMMENT;}
{multiLineComment}			{ return (int)ShaderlabToken.COMMENT;}

/**********String**********/
/*************************/
\""(\\.|[^\\""])*\""				{return (int)ShaderlabToken.STRING_LITERAL;}

/**********White Space********/
/******************************/
{white_space}                    {/* Ignore */}

/**********Keywords**********/
/***************************/
({white_space}|{symbol}){HLSLCGBlockKeyWords}({white_space}|{symbol})				    {return (int)ShaderlabToken.HLSLCGKEYWORD;}
({white_space}|{symbol}){HLSLCGNonBlockKeyWords}({white_space}|{symbol})				{return (int)ShaderlabToken.HLSLCGKEYWORD;}
:({blank})*{HLSLCGSpecialKeyWords}({white_space}|{symbol})								{return (int)ShaderlabToken.HLSLCGKEYWORDSPECIAL;}
({white_space}|{symbol}){UNITYBuiltinKeywords}{blank}*({white_space}|{symbol})		    {return (int)ShaderlabToken.UNITYKEYWORD;}
({white_space}|{symbol})#({UNITYBuiltinKeywords}|{HLSLCGBlockKeyWords})({blank}+{words})*{white_space}	    {return (int)ShaderlabToken.UNITYKEYWORD_PARA;}
({white_space}|{symbol}){UNITYBuiltinValues}({white_space}|{symbol})					{return (int)ShaderlabToken.UNITYVALUES;}

/**********Date Type**********/
/*****************************/
({white_space}|{symbol}){HLSLCGDatatypes}({white_space}|{symbol})		    {return (int)ShaderlabToken.HLSLCGDATATYPE;}
({white_space}|{symbol}){UNITYBuiltinDataTypes}({white_space}|{symbol})	    {return (int)ShaderlabToken.UNITYDATATYPE;}

/**********MACROS**********/
/*****************************/
({white_space}|{symbol}){UNITYBuiltinMacros}({white_space}|{symbol})        {return (int)ShaderlabToken.UNITYMACROS;}

/**********Function**********/
/*****************************/
({white_space}|{symbol}){HLSLCGFunctions}{blank}*\(		            {return (int)ShaderlabToken.HLSLCGFUNCTION;}
({white_space}|{symbol}){UNITYBuiltinFunctions}{blank}*\(           {return (int)ShaderlabToken.UNITYFUNCTION;}

%%
";

        static void Main(string[] args)
        {
            string convertedFileName = "Shaerlab.lex";

            if (args.Length > 0)
            {
                convertedFileName = args[0].Trim();
            }

            string lex = lexFormat;
            lex = lex.Replace("{$HLSLCGBlockKeyWords$}", string.Join("|", ShaderlabDataManager.Instance.HLSLCGBlockKeywords).ToLower());
            lex = lex.Replace("{$HLSLCGNonBlockKeyWords$}", string.Join("|", ShaderlabDataManager.Instance.HLSLCGNonblockKeywords).ToLower());
            lex = lex.Replace("{$HLSLCGSpecialKeyWords$}", string.Join("|", ShaderlabDataManager.Instance.HLSLCGSpecialKeywords).ToLower());
            lex = lex.Replace("{$HLSLCGDatatypes$}", string.Join("|", ShaderlabDataManager.Instance.HLSLCGDatatypes).ToLower());
            lex = lex.Replace("{$HLSLCGFunctions$}", string.Join("|", ShaderlabDataManager.Instance.HLSLCGFunctions.Select(f => f.Name).ToList()).ToLower());
            lex = lex.Replace("{$UNITYBuiltinDataTypes$}", string.Join("|", ShaderlabDataManager.Instance.UnityBuiltinDatatypes.Select(d => d.Name).ToList()).ToLower());
            lex = lex.Replace("{$UNITYBuiltinFunctions$}", string.Join("|", ShaderlabDataManager.Instance.UnityBuiltinFunctions.Select(f => f.Name).ToList()).ToLower());
            lex = lex.Replace("{$UNITYBuiltinMacros$}", string.Join("|", ShaderlabDataManager.Instance.UnityBuiltinMacros.Select(m => m.Name).ToList()).ToLower());
            lex = lex.Replace("{$UNITYBuiltinValues$}", string.Join("|", ShaderlabDataManager.Instance.UnityBuiltinValues.Select(v => v.Name).ToList()).ToLower());
            lex = lex.Replace("{$UNITYBuiltinKeywords$}", string.Join("|", ShaderlabDataManager.Instance.UnityKeywords.Select(k => k.Name).ToList()).ToLower());

            File.WriteAllText(convertedFileName, lex);
        }
    }
}
