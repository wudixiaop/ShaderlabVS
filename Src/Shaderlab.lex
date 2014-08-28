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
exponent            [Ee]("+"|"-")?{digit}+
floatsuffix         [fFhH]
hexdigit            [0-9a-fA-F]
float1				("+"|"-")?{digit}*"."{digit}+{floatsuffix}?
float2				("+"|"-")?{digit}+"."{digit}+{floatsuffix}?
numbers				(("+"|"-")?{digit}+|{float1}|{float2})

symbol			("+"|"-"|"*"|"/"|"%"|"||"|"<"|">"|"="|"!"|";"|","|"="|"&"|";"|"{"|"}"|","|"("|")"|"["|"]")


CmntStart			\/\*
CmntEnd				\*\/
words				[^\*\n]*
blank				" "
white_space         [ \t\v\n\f\r]


keyword				BlendState|break|Buffer|cbuffer|class|compile|const|continue|DepthStencilState|DepthStencilView|discard|do|else|extern|false|for|GeometryShader|if|in|inline|inout|interface|namespace|linear|centroid|nointerpolation|noperspective|sample|out|pass|PixelShader|precise|RasterizerState|RenderTargetView|return|register|Sampler|Sampler1D|Sampler2D|Sampler3D|SamplerCUBE|SamplerState|SamplerComparisonState|shared|Stateblock|StateblockState|static|struct|switch|tbuffer|technique|technique10|technique11|texture|Texture1D|Texture1DArray|Texture2D|Texture2DArray|Texture2DMS|Texture2DMSArray|Texture3D|TextureCube|TextureCubeArray|true|typedef|uniform|VertexShader|void|volatile|while|numthreads
keywordsSpeical     ALPHATOCOVERAGEENABLE|BLENDENABLE|SRCBLEND|DESTBLEND|BLENDOP|SRCBLENDALPHA|DESTBLENDALPHA|BLENDOPALPHA|RENDERTARGETWRITEMASK|DEPTHENABLE|DEPTHWRITEMASK|DEPTHFUNC|STENCILENABLE|STENCILREADMASK|STENCILWRITEMASK|FRONTFACESTENCILFAIL|FRONTFACESTENCILZFAIL|FRONTFACESTENCILPASS|FRONTFACESTENCILFUNC|BACKFACESTENCILFAIL|BACKFACESTENCILZFAIL|BACKFACESTENCILPASS|BACKFACESTENCILFUNC|FILLMODE|CULLMODE|FRONTCOUNTERCLOCKWISE|DEPTHBIAS|DEPTHBIASCLAMP|SLOPESCALEDDEPTHBIAS|ZCLIPENABLE|SCISSORENABLE|MULTISAMPLEENABLE|ANTIALIASEDLINEENABLE|FILTER|ADDRESSU|ADDRESSV|ADDRESSW|MIPLODBIAS|MAXANISOTROPY|COMPARISONFUNC|BORDERCOLOR|MINLOD|MAXLOD|BINORMAL|BINORMAL0|BINORMAL1|BINORMAL10|BINORMAL11|BINORMAL2|BINORMAL3|BINORMAL4|BINORMAL5|BINORMAL6|BINORMAL7|BINORMAL8|BINORMAL9|BLENDINDICES|BLENDINDICES0|BLENDINDICES1|BLENDINDICES10|BLENDINDICES11|BLENDINDICES2|BLENDINDICES3|BLENDINDICES4|BLENDINDICES5|BLENDINDICES6|BLENDINDICES7|BLENDINDICES8|BLENDINDICES9|BLENDWEIGHTS|BLENDWEIGHTS0|BLENDWEIGHTS1|BLENDWEIGHTS10|BLENDWEIGHTS11|BLENDWEIGHTS2|BLENDWEIGHTS3|BLENDWEIGHTS4|BLENDWEIGHTS5|BLENDWEIGHTS6|BLENDWEIGHTS7|BLENDWEIGHTS8|BLENDWEIGHTS9|COLOR|COLOR0|COLOR1|COLOR10|COLOR11|COLOR12|COLOR13|COLOR14|COLOR15|COLOR2|COLOR3|COLOR4|COLOR5|COLOR6|COLOR7|COLOR8|COLOR9|DIFFUSE|DIFFUSE0|DIFFUSE1|DIFFUSE10|DIFFUSE11|DIFFUSE2|DIFFUSE3|DIFFUSE4|DIFFUSE5|DIFFUSE6|DIFFUSE7|DIFFUSE8|DIFFUSE9|FOG|NORMAL|NORMAL0|NORMAL1|NORMAL10|NORMAL11|NORMAL2|NORMAL3|NORMAL4|NORMAL5|NORMAL6|NORMAL7|NORMAL8|NORMAL9|POSITION|POSITION0|POSITION1|POSITION10|POSITION11|POSITION2|POSITION3|POSITION4|POSITION5|POSITION6|POSITION7|POSITION8|POSITION9|PSIZE|PSIZE0|PSIZE1|PSIZE10|PSIZE11|PSIZE2|PSIZE3|PSIZE4|PSIZE5|PSIZE6|PSIZE7|PSIZE8|PSIZE9|SPECULAR|SPECULAR0|SPECULAR1|SPECULAR10|SPECULAR11|SPECULAR2|SPECULAR3|SPECULAR4|SPECULAR5|SPECULAR6|SPECULAR7|SPECULAR8|SPECULAR9|SV_CLIPDISTANCE|SV_CLIPDISTANCE1|SV_CLIPDISTANCE10|SV_CLIPDISTANCE11|SV_CLIPDISTANCE2|SV_CLIPDISTANCE3|SV_CLIPDISTANCE4|SV_CLIPDISTANCE5|SV_CLIPDISTANCE6|SV_CLIPDISTANCE7|SV_CLIPDISTANCE8|SV_CLIPDISTANCE9|SV_COVERAGE|SV_CULLDISTANCE|SV_CULLDISTANCE1|SV_CULLDISTANCE10|SV_CULLDISTANCE11|SV_CULLDISTANCE2|SV_CULLDISTANCE3|SV_CULLDISTANCE4|SV_CULLDISTANCE5|SV_CULLDISTANCE6|SV_CULLDISTANCE7|SV_CULLDISTANCE8|SV_CULLDISTANCE9|SV_DEPTH|SV_DISPATCHTHREADID|SV_DOMAINLOCATION|SV_GROUPID|SV_GROUPINDEX|SV_GROUPTHREADID|SV_GSINSTANCEID|SV_INSIDETESSFACTOR|SV_INSTANCEID|SV_ISFRONTFACE|SV_OUTPUTCONTROLPOINTID|SV_POSITION|SV_PRIMITIVEID|SV_RENDERTARGETARRAYINDEX|SV_SAMPLEINDEX|SV_TARGET0|SV_TARGET1|SV_TARGET2|SV_TARGET3|SV_TARGET4|SV_TARGET5|SV_TARGET6|SV_TARGET7|SV_TESSFACTOR|SV_VERTEXID|SV_VIEWPORTARRAYINDEX|TANGENT|TANGENT0|TANGENT1|TANGENT10|TANGENT11|TANGENT2|TANGENT3|TANGENT4|TANGENT5|TANGENT6|TANGENT7|TANGENT8|TANGENT9|TESSFACTOR|TESSFACTOR0|TESSFACTOR1|TESSFACTOR10|TESSFACTOR11|TESSFACTOR2|TESSFACTOR3|TESSFACTOR4|TESSFACTOR5|TESSFACTOR6|TESSFACTOR7|TESSFACTOR8|TESSFACTOR9|TEXCOORD|TEXCOORD0|TEXCOORD1|TEXCOORD10|TEXCOORD11|TEXCOORD12|TEXCOORD13|TEXCOORD14|TEXCOORD15|TEXCOORD19|TEXCOORD2|TEXCOORD3|TEXCOORD4|TEXCOORD5|TEXCOORD6|TEXCOORD7|TEXCOORD8|TEXCOORD9|VFACE|VPOS|REGISTER
datatype			AppendStructuredBuffer|bool|bool2|bool3|bool4|Buffer|ByteAddressBuffer|ConsumeStructuredBuffer|double|double2|double3|double4|fixed|fixed2|fixed3|fixed4|float|float1x1|float1x2|float1x3|float1x4|float2|float2x1|float2x2|float2x3|float2x4|float3|float3x1|float3x2|float3x3|float3x4|float4|float4x1|float4x2|float4x3|float4x4|half|half2|half3|half4|InputPatch|int|int2|int3|int4|line|lineadj|LineStream|matrix|OutputPatch|point|PointStream|RWBuffer|RWByteAddressBuffer|RWStructuredBuffer|RWTexture1D|RWTexture1DArray|RWTexture2D|RWTexture2DArray|RWTexture3D|string|StructuredBuffer|Texture1D|Texture1DArray|Texture2D|Texture2DArray|Texture2DMS|Texture2DMSArray|Texture3D|triangle|triangleadj|TriangleStream|uint|uint2|uint3|uint4|vector|sampler|sampler2D|sampler3D|samplerCUBE
function		    abs|acos|all|AllMemoryBarrier|AllMemoryBarrierWithGroupSync|any|asdouble|asfloat|asin|asint|asuint|atan|atan2|ceil|clamp|clip|cos|cosh|countbits|cross|D3DCOLORtoUBYTE4|ddx|ddx_coarse|ddx_fine|ddy|ddy_coarse|ddy_fine|degrees|determinant|DeviceMemoryBarrier|DeviceMemoryBarrierWithGroupSync|distance|dot|dst|EvaluateAttributeAtCentroid|EvaluateAttributeAtSample|EvaluateAttributeSnapped|exp|exp2|f16tof32|f32tof16|faceforward|firstbithigh|firstbitlow|floor|fmod|frac|frexp|fwidth|GetRenderTargetSampleCount|GetRenderTargetSamplePosition|GroupMemoryBarrier|GroupMemoryBarrierWithGroupSync|InterlockedAdd|InterlockedAnd|InterlockedCompareExchange|InterlockedCompareStore|InterlockedExchange|InterlockedMax|InterlockedMin|InterlockedOr|InterlockedXor|isfinite|isinf|isnan|ldexp|length|lerp|lit|log|log10|log2|mad|max|min|modf|mul|noise|normalize|pow|Process2DQuadTessFactorsAvg|Process2DQuadTessFactorsMax|Process2DQuadTessFactorsMin|ProcessIsolineTessFactors|ProcessQuadTessFactorsAvg|ProcessQuadTessFactorsMax|ProcessQuadTessFactorsMin|ProcessTriTessFactorsAvg|ProcessTriTessFactorsMax|ProcessTriTessFactorsMin|radians|rcp|reflect|refract|reversebits|round|rsqrt|saturate|sign|sin|sincos|sinh|smoothstep|sqrt|step|tan|tanh|tex1D|tex1Dbias|tex1Dgrad|tex1Dlod|tex1Dproj|tex2D|tex2Dbias|tex2Dgrad|tex2Dlod|tex2Dproj|tex3D|tex3Dbias|tex3Dgrad|tex3Dlod|tex3Dproj|texCUBE|texCUBEbias|texCUBEgrad|texCUBElod|texCUBEproj|transpose|trunc

unityDataType				Color|2D|Rect|Cube|Float|[Rr]ange{blank}*\({blank}*{numbers}{blank}*,{blank}*{numbers}{blank}*\)|Vector|appdata_base|appdata_tan|appdata_full|appdata_img|SurfaceOutput
unityBuiltinVars			UNITY_MATRIX_MVP|UNITY_MATRIX_MV|UNITY_MATRIX_V|UNITY_MATRIX_P|UNITY_MATRIX_VP|UNITY_MATRIX_T_MV|UNITY_MATRIX_IT_MV|UNITY_MATRIX_TEXTURE0|UNITY_MATRIX_TEXTURE1|UNITY_MATRIX_TEXTURE2|UNITY_MATRIX_TEXTURE3|UNITY_LIGHTMODEL_AMBIENT
unityKeywordsWithPara		#pragma|#include
unityBlockKeywords			CGPROGRAM|ENDCG|Shader|SubShader|FallBack|Properties|Pass|UsePass|Tags|BindChannels|Stencil|Material|GrabPass
unityNonBlockKeywords		LOD|Lighting|SetTexture|Cull|Fog|ZWrite|ZTest|AlphaTest|Blend|ColorMask|Offset|SeparateSpecular|ColorMaterial|Diffuse|Ambient|Shininess|Specular|Emission|[Cc]ombine|Mode|Density|Range|Greater|GEqual|Less|LEqual|Equal|NotEqual|Always|Never|Add|Sub|RevSub|Min|Max|LogicalClear|LogicalSet|LogicalCopy|LogicalCopyInverted|LogicalNoop|LogicalInvert|LogicalAnd|LogicalNand|LogicalOr|LogicalNor|LogicalXor|LogicalEquiv|LogicalAndReverse|LogicalAndInverted|LogicalOrReverse|LogicalOrInverted|One|Zero|SrcColor|SrcAlpha|DstColor|DstAlpha|OneMinusSrcColor|OneMinusSrcAlpha|OneMinusDstColor|OneMinusDstAlpha|LightMode|Always|ForwardBase|ForwardAdd|PrepassBase|PrepassFinal|Vertex|VertexLMRGBM|VertexLM|ShadowCaster|ShadowCollector|RequireOptions|SoftVegetation|Name|Bind|Ref|ReadMask|WriteMask|Comp|Fail|ZFail|DOUBLE
unityFunction				UnpackNormal|UNITY_DECLARE_SHADOWMAP|UNITY_SAMPLE_SHADOW|UNITY_SAMPLE_SHADOW_PROJ|CBUFFER_START|WorldSpaceViewDir|ObjSpaceViewDir|ParallaxOffset|Luminance|DecodeLightmap|EncodeFloatRGBA|DecodeFloatRGBA|EncodeFloatRG|EncodeViewNormalStereo|DecodeViewNormalStereo|WorldSpaceLightDir|ObjSpaceLightDir|Shade4PointLights|ShadeVertexLights
%%

/*********Comments********/
/*************************/
"//"(.)*						{return (int)ShaderlabToken.COMMENT_LINE;}
{CmntStart}{words}\**{CmntEnd}  { return (int)ShaderlabToken.COMMENT;}
{CmntStart}{words}\**           { BEGIN(COMMENT); return (int)ShaderlabToken.COMMENT;}
<COMMENT>\n                     |                                
<COMMENT>{words}\**             { return (int)ShaderlabToken.COMMENT;}
<COMMENT>{words}\**{CmntEnd}    { BEGIN(INITIAL); return (int)ShaderlabToken.COMMENT;}

/**********String**********/
/*************************/
\"(\\.|[^\\"])*\"				{return (int)ShaderlabToken.STRING_LITERAL;}

/**********White Space********/
/******************************/
{white_space}                    {/* Ignore */}

/**********Keywords**********/
/***************************/
({white_space}|{symbol}){keyword}({white_space}|{symbol})							{return (int)ShaderlabToken.KEYWORD;}
:({blank})*{keywordsSpeical}({white_space}|{symbol})								{return (int)ShaderlabToken.KEYWORDSPECIAL;}
({white_space}|{symbol}){unityBlockKeywords}{blank}*({white_space}|{symbol})		{return (int)ShaderlabToken.UNITYBLOCKKEYWORD;}
({white_space}|{symbol}){unityNonBlockKeywords}{blank}*({white_space}|{symbol})		{return (int)ShaderlabToken.UNITYNONBLOCKKEYWORD;}
({white_space}|{symbol}){unityKeywordsWithPara}({blank}+{words})*{white_space}		{return (int)ShaderlabToken.UNITYNONBLOCKKEYWORD;}
({white_space}|{symbol}){unityBuiltinVars}({white_space}|{symbol})					{return (int)ShaderlabToken.KEYWORD;}

/**********Date Type**********/
/*****************************/
({white_space}|{symbol}){datatype}({white_space}|{symbol})		  {return (int)ShaderlabToken.DATATYPE;}
({white_space}|{symbol}){unityDataType}({white_space}|{symbol})	  {return (int)ShaderlabToken.DATATYPE;}

/**********Function**********/
/*****************************/
({white_space}|{symbol}){function}{blank}*\(		 {return (int)ShaderlabToken.FUNCTION;}
({white_space}|{symbol}){unityFunction}{blank}*\(    {return (int)ShaderlabToken.UNITYFUNCTION;}

%%