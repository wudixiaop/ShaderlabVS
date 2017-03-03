
%namespace ShaderlabVS.Lexer
%option verbose, summary, noparser, nofiles, unicode
%using System.Runtime.Serialization.Formatters.Binary;
%using System.Xml.Serialization;
%using System.Linq;

/**********************************************************************************/
/********************************User Defined Code*********************************/
/**********************************************************************************/

%{
        public static void GenerateTableData()
        {
            List<TableWrapper> dataCopied = new List<TableWrapper>();
            foreach (var item in NxS)
            {
                dataCopied.Add(TableToWrapper(item));
            }

            XmlSerializer serializer = new XmlSerializer(typeof(List<TableWrapper>));
            serializer.Serialize(new FileStream("lex.xml", FileMode.OpenOrCreate), dataCopied);
        }

    public static void LoadTableDataFromLex()
    {
        string currentAssemblyDir = (new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().CodeBase.Substring(8))).DirectoryName;
        string dataPath = Path.Combine(currentAssemblyDir, "lex.xml");
        if (File.Exists(dataPath))
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<TableWrapper>));
            var datalist = serializer.Deserialize(new FileStream(dataPath, FileMode.Open)) as List<TableWrapper>;
            if (datalist != null)
            {
                NxS = datalist.Select(d => WrapperToTable(d)).ToArray();
            }
        }
    }

    private static TableWrapper TableToWrapper(Table table)
    {
        return new TableWrapper()
        {
            min = table.min,
            rng = table.rng,
            dflt = table.dflt,
            nxt = table.nxt
        };
    }

    private static Table WrapperToTable(TableWrapper wrapper)
    {
        return new Table(wrapper.min, wrapper.rng, wrapper.dflt, wrapper.nxt);
    }

    [Serializable]
    public class TableWrapper
    {
        public int min;
        public int rng;
        public int dflt;
        public short[] nxt;

        public TableWrapper()
        {

        }
    }

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

symbol				("+"|"-"|"*"|"/"|"%"|"|"|"<"|">"|"="|"!"|";"|","|"="|"&"|";"|"{"|"}"|","|"("|")"|"["|"]"|"\*")

words				[^\*\n]*
blank				" "
white_space         [ \t\v\n\f\r]

singleLineComment	"//"[^\n]*
multiLineComment	"/*"[^*]*\*(\*|([^*/]([^*])*\*))*\/

HLSLCGBlockKeyWords	        do|else|for|if|switch|while|ifdef|endif|elif|define|defined|ifndef
HLSLCGNonBlockKeyWords	    blendstate|buffer|depthstencilstate|depthstencilview|geometryshader|pixelshader|rasterizerstate|rendertargetview|samplercomparisonstate|samplerstate|stateblock|stateblockstate|texture1d|texture1darray|texture2d|texture2darray|texture2dms|texture2dmsarray|texture3d|texturecube|texturecubearray|vertexshader|break|cbuffer|centroid|class|compile|const|continue|discard|extern|false|in|inline|inout|interface|linear|namespace|nointerpolation|noperspective|numthreads|out|pass|precise|register|return|sample|shared|static|struct|tbuffer|technique|technique10|technique11|texture|true|typedef|uniform|void|volatile
HLSLCGSpecialKeyWords       addressu|addressv|addressw|alphatocoverageenable|antialiasedlineenable|backfacestencilfail|backfacestencilfunc|backfacestencilpass|backfacestencilzfail|binormal|binormal0|binormal1|binormal10|binormal11|binormal2|binormal3|binormal4|binormal5|binormal6|binormal7|binormal8|binormal9|blendenable|blendindices|blendindices0|blendindices1|blendindices10|blendindices11|blendindices2|blendindices3|blendindices4|blendindices5|blendindices6|blendindices7|blendindices8|blendindices9|blendop|blendopalpha|blendweights|blendweights0|blendweights1|blendweights10|blendweights11|blendweights2|blendweights3|blendweights4|blendweights5|blendweights6|blendweights7|blendweights8|blendweights9|bordercolor|color|color0|color1|color10|color11|color12|color13|color14|color15|color2|color3|color4|color5|color6|color7|color8|color9|comparisonfunc|depthbias|depthbiasclamp|depthenable|depthfunc|depthwritemask|destblendalpha|diffuse|diffuse0|diffuse1|diffuse10|diffuse11|diffuse2|diffuse3|diffuse4|diffuse5|diffuse6|diffuse7|diffuse8|diffuse9|fillmode|cullmode|filter|fog|frontcounterclockwise|frontfacestencilfail|frontfacestencilfunc|frontfacestencilpass|frontfacestencilzfail|maxanisotropy|maxlod|minlod|miplodbias|multisampleenable|normal|normal0|normal1|normal10|normal11|normal2|normal3|normal4|normal5|normal6|normal7|normal8|normal9|position|position0|position1|position10|position11|position2|position3|position4|position5|position6|position7|position8|position9|psize|psize0|psize1|psize10|psize11|psize2|psize3|psize4|psize5|psize6|psize7|psize8|psize9|register|rendertargetwritemask|scissorenable|slopescaleddepthbias|specular|specular0|specular1|specular10|specular11|specular2|specular3|specular4|specular5|specular6|specular7|specular8|specular9|srcblendalpha|srcblenddestblend|stencilenable|stencilreadmask|stencilwritemask|sv_clipdistance|sv_clipdistance1|sv_clipdistance10|sv_clipdistance11|sv_clipdistance2|sv_clipdistance3|sv_clipdistance4|sv_clipdistance5|sv_clipdistance6|sv_clipdistance7|sv_clipdistance8|sv_clipdistance9|sv_coverage|sv_culldistance|sv_culldistance1|sv_culldistance10|sv_culldistance11|sv_culldistance2|sv_culldistance3|sv_culldistance4|sv_culldistance5|sv_culldistance6|sv_culldistance7|sv_culldistance8|sv_culldistance9|sv_depth|sv_dispatchthreadid|sv_domainlocation|sv_groupid|sv_groupindex|sv_groupthreadid|sv_gsinstanceid|sv_insidetessfactor|sv_instanceid|sv_isfrontface|sv_outputcontrolpointid|sv_position|sv_primitiveid|sv_rendertargetarrayindex|sv_sampleindex|sv_target0|sv_target1|sv_target2|sv_target3|sv_target4|sv_target5|sv_target6|sv_target7|sv_tessfactor|sv_vertexid|sv_viewportarrayindex|tangent|tangent0|tangent1|tangent10|tangent11|tangent2|tangent3|tangent4|tangent5|tangent6|tangent7|tangent8|tangent9|tessfactor|tessfactor0|tessfactor1|tessfactor10|tessfactor11|tessfactor2|tessfactor3|tessfactor4|tessfactor5|tessfactor6|tessfactor7|tessfactor8|tessfactor9|texcoord|texcoord0|texcoord1|texcoord10|texcoord11|texcoord12|texcoord13|texcoord14|texcoord15|texcoord19|texcoord2|texcoord3|texcoord4|texcoord5|texcoord6|texcoord7|texcoord8|texcoord9|vface|vpos|zclipenable
HLSLCGDatatypes			    appendstructuredbuffer|buffer|byteaddressbuffer|consumestructuredbuffer|inputpatch|linestream|outputpatch|pointstream|rwbuffer|rwbyteaddressbuffer|rwstructuredbuffer|rwtexture1d|rwtexture1darray|rwtexture2d|rwtexture2darray|rwtexture3d|structuredbuffer|texture1d|texture1darray|texture2d|texture2darray|texture2dms|texture2dmsarray|texture3d|trianglestream|bool|bool2|bool3|bool4|double|double2|double3|double4|fixed|fixed2|fixed3|fixed4|float|float1x1|float1x2|float1x3|float1x4|float2|float2x1|float2x2|float2x3|float2x4|float3|float3x1|float3x2|float3x3|float3x4|float4|float4x1|float4x2|float4x3|float4x4|half|half2|half3|half4|int|int2|int3|int4|line|lineadj|matrix|point|sampler|sampler2d|sampler3d|samplercube|string|triangle|triangleadj|uint|uint2|uint3|uint4|vector
HLSLCGFunctions		        abs|acos|all|any|asin|atan2|atan|bitcount|bitfieldextract|bitfieldinsert|bitfieldreverse|ceil|clamp|clip|cosh|cos|cross|ddx|ddy|degrees|determinant|distance|dot|exp2|exp|faceforward|findlsb|findmsb|floattointbits|floattorawintbits|floor|fmod|frac|frexp|fwidth|intbitstofloat|inverse|isfinite|isinf|isnan|ldexp|length|lerp|lit|log10|log2|log|max|min|modf|mul|normalize|pack_2half|pack_2ushort|pack_4byte|pack_4ubyte|pow|radians|reflect|refract|round|rsqrt|saturate|sign|sincos|sinh|sin|smoothstep|sqrt|step|tanh|tan|tex1darraybias|tex1darraycmpbias|tex1darraycmplod|tex1darrayfetch|tex1darraylod|tex1darray|tex1darrayproj|tex1darraysize|tex1dbias|tex1dcmpbias|tex1dcmplod|tex1dfetch|tex1dlod|tex1d|tex1dproj|tex1dsize|tex2darraybias|tex2darrayfetch|tex2darraylod|tex2darray|tex2darrayproj|tex2darraysize|tex2dbias|tex2dcmpbias|tex2dcmplod|tex2dfetch|tex2dlod|tex2dmsarrayfetch|tex2dmsarraysize|tex2dmsfetch|tex2dmssize|tex2d|tex2dproj|tex2dsize|tex3dbias|tex3dfetch|tex3dlod|tex3d|tex3dproj|tex3dsize|texbuf|texbufsize|texcubearraybias|texcubearraylod|texcubearray|texcubearraysize|texcubebias|texcubelod|texcube|texcubeproj|texcubesize|texrbuf|texrbufsize|texrectbias|texrectfetch|texrectlod|texrect|texrectproj|texrectsize|transpose|trunc|unpack_2half|unpack_2ushort|unpack_4byte|unpack_4ubyte

UNITYBuiltinDataTypes       color|2d|rect|float|range|vector|appdata_base|appdata_tan|appdata_full|appdata_img|surfaceoutput
UNITYBuiltinFunctions       unpacknormal
UNITYBuiltinMacros          unity_pass_forwardbase|unity_pass_forwardadd|unity_pass_prepassbase|unity_pass_prepassfinal|unity_pass_shadowcaster|unity_pass_shadowcollector|shader_api_opengl|shader_api_d3d9|shader_api_xbox360|shader_api_ps3|shader_api_d3d11|shader_api_gles|shader_api_flash|shader_api_d3d11_9x|shader_api_mobile|unity_proj_coord|unity_atten_channel|unity_half_texel_offset|unity_uv_starts_at_top|unity_might_not_have_depth_texture|unity_near_clip_value|unity_compiler_cg|unity_compiler_hlsl|unity_compiler_hlsl2glsl|unity_can_compile_tessellation|unity_initialize_output|unity_compiler_hlsl|unity_compiler_hlsl2glsl|unity_compiler_cg|unity_declare_shadowmap|unity_sample_shadow|unity_sample_shadow_proj|cbuffer_start|cbuffer_end|transform_tex|directional|directional_cookie|point|point_noatt|point_cookie|spot
UNITYBuiltinKeywords        pragma|include|cgprogram|endcg|shader|subshader|fallback|properties|pass|usepass|tags|bindchannels|stencil|material|grabpass|lod|lighting|settexture|cull|fog|zwrite|ztest|alphatest|blend|colormask|offset|separatespecular|colormaterial|diffuse|ambient|shininess|specular|emission|combine|mode|density|srccolor|srcalpha|dstcolor|dstalpha|oneminussrccolor|oneminussrcalpha|oneminusdstcolor|oneminusdstalpha|lightmode|always|forwardbase|forwardadd|prepassbase|prepassfinal|vertex|vertexlmrgbm|vertexlm|shadowcaster|shadowcollector|softvegetation|name|bind|ref|readmask|writemask|comp|fail|zfail|double|category|constantcolor
UNITYBuiltinValues          unity_matrix_mvp|unity_matrix_mv|unity_matrix_v|unity_matrix_p|unity_matrix_vp|unity_matrix_t_mv|unity_matrix_it_mv|unity_matrix_texture0|unity_matrix_texture1|unity_matrix_texture2|unity_matrix_texture3|_object2world|_world2object|_worldspacecamerapos|unity_scale|_modellightcolor|_specularlightcolor|_objectspacelightpos|_light2world|_world2light|_object2light|_worldspacelightpos0|_lightcolor0|unity_lightmodel_ambient|_lightmatrix0|_lighttexture0|_time|_sintime|_costime|unity_deltatime|_projectionparams|_screenparams

%%

/*********Comments********/
/*************************/
{singleLineComment}			{return (int)ShaderlabToken.COMMENT;}
{multiLineComment}			{ return (int)ShaderlabToken.COMMENT;}

/**********String**********/
/*************************/
\"(\\.|[^\\"])*\"				{return (int)ShaderlabToken.STRING_LITERAL;}

/**********White Space********/
/******************************/
{white_space}                    {/* Ignore */}

/**********Keywords**********/
/***************************/
({white_space}|{symbol}){HLSLCGBlockKeyWords}({white_space}|{symbol})				    {return (int)ShaderlabToken.HLSLCGKEYWORD;}
({white_space}|{symbol}){HLSLCGNonBlockKeyWords}({white_space}|{symbol})				{return (int)ShaderlabToken.HLSLCGKEYWORD;}
:({blank})*{HLSLCGSpecialKeyWords}({white_space}|{symbol})								{return (int)ShaderlabToken.HLSLCGKEYWORDSPECIAL;}
({white_space}|{symbol}){UNITYBuiltinKeywords}{blank}*({white_space}|{symbol})		    {return (int)ShaderlabToken.UNITYKEYWORD;}
({white_space}|{symbol})#{white_space}*({UNITYBuiltinKeywords}|{HLSLCGBlockKeyWords})({blank}+{words}|{symbol}+)*{white_space}	    {return (int)ShaderlabToken.UNITYKEYWORD_PARA;}
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
