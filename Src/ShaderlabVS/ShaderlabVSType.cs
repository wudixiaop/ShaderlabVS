using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

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
        [Name(Constants.ShaderlabKeyword)]
        internal static ClassificationTypeDefinition ShaderlabKeywordType = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(Constants.ShaderlabUnityBlockKeywords)]
        internal static ClassificationTypeDefinition ShaderlabUnityBlockKeywords = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(Constants.ShaderlabUnityNonBlockKeywords)]
        internal static ClassificationTypeDefinition ShaderlabUnityNonBlockKeywords = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(Constants.ShaderlabDataType)]
        internal static ClassificationTypeDefinition ShaderlabDataTypeType = null;


    }
}
