using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.BraceCompletion;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;
using ShaderlabVS.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ShaderlabVS
{
    #region Shaderlab Completion Source
    public class ShaderlabCompletionSource : ICompletionSource
    {
        private static HashSet<string> WordsInDocuments = new HashSet<string>();
        private ShaderlabCompletionSourceProvider sourceProvider;
        private ITextBuffer textBuffer;
        private ITextDocument textDocument;

        static ImageSource functionsImage;
        static ImageSource datatypeImage;
        static ImageSource keywordsImage;
        static ImageSource valuesImage;

        static ShaderlabCompletionSource()
        {
            functionsImage = GetImageFromAssetByName("Method.png");
            datatypeImage = GetImageFromAssetByName("Structure.png");
            keywordsImage = GetImageFromAssetByName("Keywords.png");
            valuesImage = GetImageFromAssetByName("Values.png");

        }


        public ShaderlabCompletionSource(ShaderlabCompletionSourceProvider completonSourceProvider, ITextBuffer textBuffer, ITextDocument document)
        {
            this.sourceProvider = completonSourceProvider;
            this.textBuffer = textBuffer;
            this.textDocument = document;
        }

        public static void SetWordsInDocuments(string text)
        {
            StringReader reader = new StringReader(text);

            string line = reader.ReadLine();

            while (line != null)
            {
                if (Utilities.IsCommentLine(line))
                {
                    line = reader.ReadLine();
                    continue;
                }

                string[] words = line.Split(
                    new char[] { '{', '}', ' ', '\t', '(', ')', '[', ']', '+', '-', '*', '/', '%', '^', '>', '<', ':',
                                '.', ';', '\"', '\'', '?', '\\', '&', '|', '`', '$', '#', ','},
                    StringSplitOptions.RemoveEmptyEntries);

                foreach (var word in words)
                {
                    WordsInDocuments.Add(word);
                }

                line = reader.ReadLine();
            }
        }

        public static void ClearWordsInDocuments()
        {
            WordsInDocuments.Clear();
        }

        private ITrackingSpan FindTokenSpanAtPosition(ITrackingPoint point, ICompletionSession completionSession)
        {
            SnapshotPoint ssPoint = (completionSession.TextView.Caret.Position.BufferPosition) - 1;
            ITextStructureNavigator navigator = sourceProvider.TextNavigatorService.GetTextStructureNavigator(this.textBuffer);
            TextExtent textExtent = navigator.GetExtentOfWord(ssPoint);
            return ssPoint.Snapshot.CreateTrackingSpan(textExtent.Span, SpanTrackingMode.EdgeInclusive);

        }

        public void AugmentCompletionSession(ICompletionSession session, IList<CompletionSet> completionSets)
        {
            List<Completion> completionList = new List<Completion>();

            HashSet<string> keywords = new HashSet<string>();

            // Add functions into auto completion list
            //
            ShaderlabDataManager.Instance.HLSLCGFunctions.ForEach(f =>
            {
                completionList.Add(new Completion(f.Name, f.Name, f.Description, functionsImage, null));
                keywords.Add(f.Name);
            });

            // Datatypes
            //
            ShaderlabDataManager.Instance.HLSLCGDatatypes.ForEach(d =>
            {
                completionList.Add(new Completion(d, d, "", datatypeImage, null));
                keywords.Add(d);
            });

            // Keywords
            //
            ShaderlabDataManager.Instance.HLSLCGBlockKeywords.ForEach(k =>
            {
                completionList.Add(new Completion(k, k, "", keywordsImage, null));
                keywords.Add(k);
            });

            ShaderlabDataManager.Instance.HLSLCGNonblockKeywords.ForEach(k =>
            {
                completionList.Add(new Completion(k, k, "", keywordsImage, null));
                keywords.Add(k);
            });

            ShaderlabDataManager.Instance.HLSLCGSpecialKeywords.ForEach(k =>
            {
                completionList.Add(new Completion(k, k, "", keywordsImage, null));
                keywords.Add(k);
            });

            if (textDocument != null && !Utilities.IsInCGOrHLSLFile(textDocument.FilePath))
            {
                // Unity data types
                //
                ShaderlabDataManager.Instance.UnityBuiltinDatatypes.ForEach(d =>
                {
                    completionList.Add(new Completion(d.Name, d.Name, d.Description, datatypeImage, null));
                    keywords.Add(d.Name);
                });

                // Unity Functions
                //
                ShaderlabDataManager.Instance.UnityBuiltinFunctions.ForEach(f =>
                {
                    completionList.Add(new Completion(f.Name, f.Name, f.Description, functionsImage, null));
                    keywords.Add(f.Name);
                });


                ShaderlabDataManager.Instance.UnityKeywords.ForEach(k =>
                {
                    completionList.Add(new Completion(k.Name, k.Name, k.Description, keywordsImage, null));
                    keywords.Add(k.Name);
                });

                // Unity values/enums
                ShaderlabDataManager.Instance.UnityBuiltinValues.ForEach(v =>
                {
                    completionList.Add(new Completion(v.Name, v.Name, v.VauleDescription, valuesImage, null));
                    keywords.Add(v.Name);
                });

                // Unity Macros
                // 
                ShaderlabDataManager.Instance.UnityBuiltinMacros.ForEach(m =>
                {
                    string description = string.Format("{0}\n{1}", string.Join(";\n", m.Synopsis), m.Description);
                    if (m.Synopsis.Count > 0)
                    {
                        completionList.Add(new Completion(m.Name, m.Name, description, functionsImage, null));
                    }
                    else
                    {
                        completionList.Add(new Completion(m.Name, m.Name, description, valuesImage, null));
                    }
                    keywords.Add(m.Name);
                });
            }

            // Add words in current file
            //
            foreach (var word in WordsInDocuments)
            {
                if (!keywords.Contains(word))
                {
                    completionList.Add(new Completion(word, word, string.Empty, valuesImage, null));
                }
            }

            completionSets.Add(new CompletionSet("Token", "Token", FindTokenSpanAtPosition(session.GetTriggerPoint(this.textBuffer), session), completionList, null));
        }

        private static ImageSource GetImageFromAssetByName(string imageFileName)
        {
            string currentAssemblyDir = (new FileInfo(Assembly.GetExecutingAssembly().CodeBase.Substring(8))).DirectoryName;
            return new BitmapImage(new Uri(Path.Combine(currentAssemblyDir, "Assets", imageFileName), UriKind.Absolute));
        }

        private bool isDispose = false;

        public void Dispose()
        {
            if (isDispose)
            {
                GC.SuppressFinalize(this);
                isDispose = true;
            }
        }
    }

    #endregion

    #region Shaderlab Completion Source Provider


    [Export(typeof(ICompletionSourceProvider))]
    [Name("CompletionSourceProvider")]
    [ContentType(Constants.ContentType)]
    public class ShaderlabCompletionSourceProvider : ICompletionSourceProvider, IWpfTextViewCreationListener
    {

        [Import]
        public ITextStructureNavigatorSelectorService TextNavigatorService { get; set; }

        [Import]
        public ITextDocumentFactoryService textDocumentFactoryService { get; set; }

        public void TextViewCreated(IWpfTextView textView)
        {

        }

        public ICompletionSource TryCreateCompletionSource(ITextBuffer textBuffer)
        {
            ITextDocument textDocument;
            textDocumentFactoryService.TryGetTextDocument(textBuffer, out textDocument);
            return new ShaderlabCompletionSource(this, textBuffer, textDocument);
        }
    }
    #endregion

    #region Shaderlab Completion Command Handler

    public class ShaderlabCompletionCommandHandlder : IOleCommandTarget
    {
        private IOleCommandTarget nextCommandHandler;
        private ITextView textView;
        private ShaderlabCompletionHandlerPrvider completionHandlerProvider;
        private ICompletionSession completionSession;

        public ShaderlabCompletionCommandHandlder(IVsTextView textViewAdapter, ITextView textView, ShaderlabCompletionHandlerPrvider handlerProvider)
        {
            this.textView = textView;
            this.completionHandlerProvider = handlerProvider;

            textViewAdapter.AddCommandFilter(this, out nextCommandHandler);

        }

        private bool TriggerCompletion()
        {
            SnapshotPoint? caretPoint = textView.Caret.Position.Point.GetPoint(textBuffer => !textBuffer.ContentType.IsOfType("projection"), PositionAffinity.Predecessor);
            if (caretPoint.HasValue)
            {

                completionSession = completionHandlerProvider.CompletionBroker.CreateCompletionSession(textView, caretPoint.Value.Snapshot.CreateTrackingPoint(caretPoint.Value.Position, PointTrackingMode.Positive), true);
                completionSession.Dismissed += completionSession_Dismissed;
                completionSession.Start();
                return true;
            }

            return false;
        }

        void completionSession_Dismissed(object sender, EventArgs e)
        {
            completionSession.Dismissed -= completionSession_Dismissed;
            completionSession = null;
        }

        public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            uint cmdID = nCmdID;
            char typedChar = char.MinValue;

            if (pguidCmdGroup == VSConstants.VSStd2K && nCmdID == (uint)VSConstants.VSStd2KCmdID.TYPECHAR)
            {
                typedChar = (char)(ushort)Marshal.GetObjectForNativeVariant(pvaIn);
            }

            if (nCmdID == (uint)VSConstants.VSStd2KCmdID.RETURN
                || cmdID == (uint)VSConstants.VSStd2KCmdID.TAB)
            {
                if (completionSession != null && !completionSession.IsDismissed)
                {
                    if (completionSession.SelectedCompletionSet.SelectionStatus.IsSelected)
                    {
                        completionSession.Commit();
                        return VSConstants.S_OK;
                    }
                    else
                    {
                        completionSession.Dismiss();
                    }
                }
            }

            int returnValue = nextCommandHandler.Exec(pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);

            bool isHandled = false;
            if (!typedChar.Equals(char.MinValue))
            {
                if (completionSession == null || completionSession.IsDismissed)
                {
                    TriggerCompletion();
                    if (completionSession != null)
                    {
                        completionSession.Filter();
                    }
                }
                else
                {
                    completionSession.Filter();
                }

                isHandled = true;
            }
            else if (cmdID == (uint)VSConstants.VSStd2KCmdID.BACKSPACE
                || cmdID == (uint)VSConstants.VSStd2KCmdID.DELETE)
            {
                if (completionSession != null && !completionSession.IsDismissed)
                {
                    completionSession.Filter();
                }

                isHandled = true;
            }

            if (isHandled)
            {
                return VSConstants.S_OK;
            }

            return returnValue;
        }

        public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            return nextCommandHandler.QueryStatus(pguidCmdGroup, cCmds, prgCmds, pCmdText);
        }
    }


    #endregion

    #region Shaderlab Completion Handler Provider

    [Export(typeof(IVsTextViewCreationListener))]
    [ContentType(Constants.ContentType)]
    [Name("ShaderlabCompletionHandlerPrvider")]
    [TextViewRole(PredefinedTextViewRoles.Editable)]
    public class ShaderlabCompletionHandlerPrvider : IVsTextViewCreationListener
    {
        [Import]
        public IVsEditorAdaptersFactoryService AdapterService = null;

        [Import]
        public ICompletionBroker CompletionBroker { get; set; }

        public void VsTextViewCreated(IVsTextView textViewAdapter)
        {
            ITextView textView = AdapterService.GetWpfTextView(textViewAdapter);
            if (textView == null)
            {
                return;
            }

            textView.Properties.GetOrCreateSingletonProperty(() =>
                {
                    return new ShaderlabCompletionCommandHandlder(textViewAdapter, textView, this);
                });
        }
    }

    #endregion

    #region BraceCompletion

    [Export(typeof(IBraceCompletionContextProvider))]
    [ContentType(Constants.ContentType)]
    [BracePair('(', ')')]
    [BracePair('[', ']')]
    [BracePair('{', '}')]
    [BracePair('"', '"')]
    [BracePair('\'', '\'')]
    internal sealed class ShaderlabVSBraceCompletionContextProvider : IBraceCompletionContextProvider
    {
        public bool TryCreateContext(ITextView textView, SnapshotPoint openingPoint, char openingBrace, char closingBrace, out IBraceCompletionContext context)
        {
            context = null;
            if (IsValidBraceCompletionContext(openingPoint))
            {
                context = new ShaderlabBraceCompletionContext();
                return true;
            }

            return false;
        }

        private bool IsValidBraceCompletionContext(SnapshotPoint openingPoint)
        {
            if (openingPoint.Position >= 0)
            {
                return true;
            }

            return false;
        }
    }

    [Export(typeof(IBraceCompletionContext))]
    internal sealed class ShaderlabBraceCompletionContext : IBraceCompletionContext
    {

        public bool AllowOverType(IBraceCompletionSession session)
        {
            return true;
        }

        public void Finish(IBraceCompletionSession session)
        {
        }

        public void OnReturn(IBraceCompletionSession session)
        {
        }

        public void Start(IBraceCompletionSession session)
        {
        }
    }
    #endregion
}
