using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace ShaderlabVS
{
    internal static class ShaderlabVSClassificationDefinition
    {
        /// <summary>
        /// Defines the "ShaderlabTextt" classification type.
        /// </summary>
        [Export(typeof(ClassificationTypeDefinition))]
        [Name(Constants.ShaderlabText)]
        internal static ClassificationTypeDefinition ShaderlabTextType = null;

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
        [Name(Constants.ShaderlabDataType)]
        internal static ClassificationTypeDefinition ShaderlabDataTypeType = null;

    }
}
