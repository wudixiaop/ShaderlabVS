using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Text.Tagging;
using ShaderlabVS.Lexer;
using System.Diagnostics;

namespace ShaderlabVS
{
    #region Provider definition

    /// <summary>
    /// Apply settings for all .shader files
    /// </summary>
    [Export(typeof(ITaggerProvider))]
    [ContentType(Constants.ContentType)]
    [TagType(typeof(ClassificationTag))]
    internal sealed class ShaderlabVSClassifierProvider : ITaggerProvider
    {
        [Export]
        [Name(Constants.ContentType)]
        [BaseDefinition(Constants.BaseDefination)]
        public static ContentTypeDefinition ShaderlabContentType = null;

        [Export]
        [FileExtension(Constants.ShaderFileNameExt)]
        [ContentType(Constants.ContentType)]
        public static FileExtensionToContentTypeDefinition ShaderlabFileTYpe = null;

        [Import]
        internal IClassificationTypeRegistryService classificationTypeRegistry = null;


        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            return new ShaderlabClassifier(buffer, classificationTypeRegistry) as ITagger<T>;
        }
    }
    #endregion //provider def

    #region Classifier

    internal sealed class ShaderlabClassifier : ITagger<ClassificationTag>
    {
        Dictionary<ShaderlabToken, IClassificationType> classTypeDict;
        Scanner scanner;
        ITextBuffer textBuffer;

        public ShaderlabClassifier(ITextBuffer buffer, IClassificationTypeRegistryService registerService)
        {
            textBuffer = buffer;
            scanner = new Scanner();

            classTypeDict = new Dictionary<ShaderlabToken, IClassificationType>();
            classTypeDict.Add(ShaderlabToken.TEXT, registerService.GetClassificationType(Constants.ShaderlabText));
            classTypeDict.Add(ShaderlabToken.COMMENT, registerService.GetClassificationType(Constants.ShaderlabComment));
            classTypeDict.Add(ShaderlabToken.COMMENT_LINE, registerService.GetClassificationType(Constants.ShaderlabComment));
            classTypeDict.Add(ShaderlabToken.DATATYPE, registerService.GetClassificationType(Constants.ShaderlabDataType));
            classTypeDict.Add(ShaderlabToken.KEYWORD, registerService.GetClassificationType(Constants.ShaderlabKeyword));
            classTypeDict.Add(ShaderlabToken.KEYWORDSPECIAL, registerService.GetClassificationType(Constants.ShaderlabKeyword));
            classTypeDict.Add(ShaderlabToken.UNITYBLOCKKEYWORD, registerService.GetClassificationType(Constants.ShaderlabUnityBlockKeywords));
            classTypeDict.Add(ShaderlabToken.UNITYNONBLOCKKEYWORD, registerService.GetClassificationType(Constants.ShaderlabUnityNonBlockKeywords));
            classTypeDict.Add(ShaderlabToken.STRING_LITERAL, registerService.GetClassificationType(Constants.ShaderlabStrings));
            classTypeDict.Add(ShaderlabToken.UNDEFINED, registerService.GetClassificationType(Constants.ShaderlabText));
            classTypeDict.Add(ShaderlabToken.FUNCTION, registerService.GetClassificationType(Constants.ShaderlabFunction));
            classTypeDict.Add(ShaderlabToken.UNITYFUNCTION, registerService.GetClassificationType(Constants.ShaderlabFunction));
        }

        public IEnumerable<ITagSpan<ClassificationTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            string text = spans[0].Snapshot.GetText();
            scanner.SetSource(text, 0);
            int token;
            IClassificationType cf;

            do
            {
                token = scanner.NextToken();
                int pos = scanner.GetPos();
                int length = scanner.GetLength();

                if (pos < 0 || length < 0 || pos > text.Length)
                {
                    continue;
                }

                if (pos + length > text.Length)
                {
                    length = text.Length - pos;
                }

                if (classTypeDict.TryGetValue((ShaderlabToken)token, out cf))
                {
                    switch ((ShaderlabToken)token)
                    {
                        case ShaderlabToken.KEYWORD:
                        case ShaderlabToken.UNITYBLOCKKEYWORD:
                        case ShaderlabToken.UNITYNONBLOCKKEYWORD:
                        case ShaderlabToken.KEYWORDSPECIAL:
                        case ShaderlabToken.FUNCTION:
                        case ShaderlabToken.UNITYFUNCTION:
                        case ShaderlabToken.DATATYPE:
                            pos++;
                            length = length - 2;
                            scanner.PushbackText(length + 1);
                            break;
                    }

                    yield return new TagSpan<ClassificationTag>(new SnapshotSpan(spans[0].Snapshot, new Span(pos, length)),
                                                                new ClassificationTag(cf));

                }

            } while (token > (int)Tokens.EOF);

        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
    }

    #endregion //Classifier
}
