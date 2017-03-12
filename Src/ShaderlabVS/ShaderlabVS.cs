using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using ShaderlabVS.Lexer;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

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
        public static FileExtensionToContentTypeDefinition ShaderlabFileType = null;

        [Export]
        [FileExtension(Constants.ComputeShaderFileNameExt)]
        [ContentType(Constants.ContentType)]
        public static FileExtensionToContentTypeDefinition ComputeShaderFileType = null;

        [Export]
        [FileExtension(Constants.CGIncludeFileExt)]
        [ContentType(Constants.ContentType)]
        public static FileExtensionToContentTypeDefinition CgIncludeFileType = null;

        [Export]
        [FileExtension(Constants.GLSLIncludeFileExt)]
        [ContentType(Constants.ContentType)]
        public static FileExtensionToContentTypeDefinition GLSLIncludeFileType = null;

        [Export]
        [FileExtension(Constants.CGFile)]
        [ContentType(Constants.ContentType)]
        public static FileExtensionToContentTypeDefinition cgFileType = null;

        [Export]
        [FileExtension(Constants.HLSLFile)]
        [ContentType(Constants.ContentType)]
        public static FileExtensionToContentTypeDefinition hlslFileType = null;

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

        static ShaderlabClassifier()
        {
            Scanner.LoadTableDataFromLex();
        }

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
            classTypeDict.Add(ShaderlabToken.HLSLCGDATATYPE, registerService.GetClassificationType(Constants.ShaderlabDataType));
            classTypeDict.Add(ShaderlabToken.HLSLCGFUNCTION, registerService.GetClassificationType(Constants.ShaderlabFunction));
            classTypeDict.Add(ShaderlabToken.HLSLCGKEYWORD, registerService.GetClassificationType(Constants.ShaderlabHLSLCGKeyword));
            classTypeDict.Add(ShaderlabToken.HLSLCGKEYWORDSPECIAL, registerService.GetClassificationType(Constants.ShaderlabHLSLCGKeyword));
            classTypeDict.Add(ShaderlabToken.UNITYKEYWORD, registerService.GetClassificationType(Constants.ShaderlabUnityKeywords));
            classTypeDict.Add(ShaderlabToken.UNITYKEYWORD_PARA, registerService.GetClassificationType(Constants.ShaderlabUnityKeywordsPara));
            classTypeDict.Add(ShaderlabToken.UNITYDATATYPE, registerService.GetClassificationType(Constants.ShaderlabDataType));
            classTypeDict.Add(ShaderlabToken.UNITYFUNCTION, registerService.GetClassificationType(Constants.ShaderlabFunction));
            classTypeDict.Add(ShaderlabToken.STRING_LITERAL, registerService.GetClassificationType(Constants.ShaderlabStrings));
            classTypeDict.Add(ShaderlabToken.UNITYVALUES, registerService.GetClassificationType(Constants.ShaderlabUnityKeywords));
            classTypeDict.Add(ShaderlabToken.UNDEFINED, registerService.GetClassificationType(Constants.ShaderlabText));
        }

        public IEnumerable<ITagSpan<ClassificationTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            ShaderlabCompletionSource.ClearWordsInDocuments();
            ShaderlabCompletionSource.SetWordsInDocuments(spans[0].Snapshot.GetText());
            
            string text = " " + spans[0].Snapshot.GetText().ToLower();
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
                        case ShaderlabToken.HLSLCGKEYWORD:
                        case ShaderlabToken.UNITYKEYWORD:
                        case ShaderlabToken.UNITYKEYWORD_PARA:
                        case ShaderlabToken.HLSLCGDATATYPE:
                        case ShaderlabToken.HLSLCGFUNCTION:
                        case ShaderlabToken.UNITYFUNCTION:
                        case ShaderlabToken.UNITYMACROS:
                        case ShaderlabToken.UNITYDATATYPE:
                        case ShaderlabToken.UNITYVALUES:
                            length = length - 2;
                            scanner.PushbackText(length + 1);
                            break;
                        case ShaderlabToken.HLSLCGKEYWORDSPECIAL:
                            pos--;
                            length = length - 1;
                            scanner.PushbackText(length);
                            break;
                        case ShaderlabToken.STRING_LITERAL:
                        case ShaderlabToken.COMMENT:
                            pos--;
                            break;
                    }

                    if (pos < 0 || length < 0 || pos > text.Length)
                    {
                        continue;
                    }

                    yield return new TagSpan<ClassificationTag>(new SnapshotSpan(spans[0].Snapshot, new Span(pos, length)),
                                                                new ClassificationTag(cf));

                }

            } while (token > (int)Tokens.EOF);

        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged
        {
            add { }
            remove { }
        }
    }

    #endregion //Classifier
}
