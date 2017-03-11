using Microsoft.VisualStudio;
using System;

namespace ShaderlabVS
{
    internal class Constants
    {
        public const string ShaderlabText = "Shaderlab-Text";
        public const string ShaderlabStrings = "Shaderlab-Strings";
        public const string ShaderlabHLSLCGKeyword = "Shaderlab-HLSLCGKeywords";
        public const string ShaderlabUnityKeywords = "Shaderlab-UnityKeywords";
        public const string ShaderlabUnityKeywordsPara = "Shaderlab-UnityKeywordsPara";
        public const string ShaderlabFunction = "Shaderlab-Fucntion";
        public const string ShaderlabComment = "Shaderlab-Comment";
        public const string ShaderlabDataType = "Shaderlab-DataType";
        public const string ShaderlabBracesMarker = "Shaderlab-BraceMarker";
        public const string ShaderlabShortcuts = "Shaderlab-Shorts";

        public const string ShaderFileNameExt = ".shader";
        public const string ComputeShaderFileNameExt = ".compute";
        public const string CGIncludeFileExt = ".cginc";
        public const string GLSLIncludeFileExt = ".glslinc";
        public const string CGFile = ".cg";
        public const string HLSLFile = ".hlsl";
        public const string ContentType = "ShaderlabVS";
        public const string BaseDefination = "code";

        public static readonly Guid ShaderlabGuid = new Guid("c702cfb7-573c-45f4-9469-115fcb519ad2");
        public static readonly Guid VS97CmdGuid = typeof(VSConstants.VSStd97CmdID).GUID;
        public static readonly Guid VSStd2KcmdGuid = typeof(VSConstants.VSStd2KCmdID).GUID;
    }
}
