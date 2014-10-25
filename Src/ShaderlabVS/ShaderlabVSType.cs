using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

namespace ShaderlabVS
{
    internal static class ShaderlabVSClassificationDefinition
    {
        [Export(typeof(ClassificationTypeDefinition))]
        [Name(Constants.ShaderlabText)]
        internal static ClassificationTypeDefinition ShaderlabTextType = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(Constants.ShaderlabStrings)]
        internal static ClassificationTypeDefinition ShaderlabStrings = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(Constants.ShaderlabFunction)]
        internal static ClassificationTypeDefinition ShaderlabFunctionType = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(Constants.ShaderlabComment)]
        internal static ClassificationTypeDefinition ShaderlabCommentType = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(Constants.ShaderlabHLSLCGKeyword)]
        internal static ClassificationTypeDefinition ShaderlabKeywordType = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(Constants.ShaderlabUnityKeywords)]
        internal static ClassificationTypeDefinition ShaderlabUnityKeywordsType = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(Constants.ShaderlabUnityKeywordsPara)]
        internal static ClassificationTypeDefinition ShaderlabUnityKeywordsParaType = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(Constants.ShaderlabDataType)]
        internal static ClassificationTypeDefinition ShaderlabDataTypeType = null;


    }
}
