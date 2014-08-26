%namespace ShaderlabVS.Lexer
%option verbose, summary, noparser, nofiles, unicode

/**********************************************************************************/
/********************************User Defined Code*********************************/
/**********************************************************************************/

%{
     public int NextToken() { return yylex(); }
     public int GetPos() { return yypos; }
     public int GetLength() { return yyleng; }
%}

/********************************Rules Section*********************************/

%x COMMENT
digit               [0-9]
alpha               [a-zA-Z_]
exponent            [Ee]("+"|"-")?{digit}+
floatsuffix         [fFhH]
white_space         [ \t\v\n\f\r]
hexdigit            [0-9a-fA-F]
CmntStart			\/\*
CmntEnd				\*\/
words				[^\*\n]*
keyword				return|if|else|while|do|for|foreach|break|continue|switch|case|default|goto|class|struct|enum|extern|interface|namespace|public|static|this|volatile|using|in|out|true|false
datatype			bool|int|float|float2|float3|float4|fixed|fixed2|fixed3|fixed4|half|half2|half3|half4|bvec|ivec|vec|vec2|vec3|vec4|mat2|mat3|mat4|sampler1D|sampler2D|sampler3D|samplerCube	

unitykeywords		#pragma

%%

/*********Comments********/
/*************************/

"//"(.)*                    {return (int)ShaderlabToken.COMMENT_LINE;}

{CmntStart}{words}\**{CmntEnd} { return (int)ShaderlabToken.COMMENT;}
{CmntStart}{words}\**          { BEGIN(COMMENT); return (int)ShaderlabToken.COMMENT;}
<COMMENT>\n                     |                                
<COMMENT>{words}\**            { return (int)ShaderlabToken.COMMENT;}
<COMMENT>{words}\**{CmntEnd}   { BEGIN(INITIAL); return (int)ShaderlabToken.COMMENT;}

/**********Numbers********/
/*************************/

"0x"{hexdigit}+                {return (int)ShaderlabToken.NUMBER;}
{digit}+                {return (int)ShaderlabToken.NUMBER;}
{digit}+{exponent}            {return (int)ShaderlabToken.FLOAT;}
{digit}*"."{digit}+({exponent})?({floatsuffix})?    {return (int)ShaderlabToken.FLOAT;}
{digit}+"."{digit}*({exponent})?({floatsuffix})?    {return (int)ShaderlabToken.FLOAT;}

/**********String**********/
/*************************/

\"(\\.|[^\\"])*\"            {return (int)ShaderlabToken.STRING_LITERAL;}

/**********White Space && Unrecognized********/
/*********************************************/
{white_space}                    {/* Ignore */}
.                    {return (int)ShaderlabToken.UNDEFINED;}

/**********Keywords**********/
/***************************/
{white_space}{keyword}{white_space}	{return (int)ShaderlabToken.KEYWORD;}

{white_space}{unitykeywords}{white_space}{words} {return (int)ShaderlabToken.KEYWORD;}

/**********Date Type**********/
/***************************/
{white_space}{datatype}{white_space}  {return (int)ShaderlabToken.DATATYPE;}

%%