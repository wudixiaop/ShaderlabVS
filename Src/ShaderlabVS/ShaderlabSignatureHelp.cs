using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;
using ShaderlabVS.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.InteropServices;

namespace ShaderlabVS
{
    //
    // REFERENCES: http://msdn.microsoft.com/zh-cn/library/vstudio/ee334194%28v=vs.110%29.aspx
    //


    #region Parameter

    internal class Parameter : IParameter
    {

        public Parameter(string documentation, Span locus, string name, ISignature signature)
        {
            Documentation = documentation;
            Locus = locus;
            Name = name;
            Signature = signature;
        }

        public string Documentation
        {
            get;
            private set;
        }

        public Span Locus
        {
            get;
            private set;
        }

        public string Name
        {
            get;
            private set;
        }

        public Span PrettyPrintedLocus
        {
            get;
            private set;
        }

        public ISignature Signature
        {
            get;
            private set;
        }
    }

    internal class Signature : ISignature
    {
        private ITrackingSpan applicationToSpan;
        public ITrackingSpan ApplicableToSpan
        {
            get
            {
                return applicationToSpan;
            }
            set
            {
                applicationToSpan = value;
            }
        }

        private string content;
        public string Content
        {
            get
            {
                return content;
            }
            set
            {
                content = value;
            }
        }

        private IParameter currentParameter;
        public IParameter CurrentParameter
        {
            get
            {
                return currentParameter;
            }
            set
            {
                if (currentParameter != value)
                {
                    var previous = currentParameter;
                    currentParameter = value;
                    RaiseParameterChanged(previous, currentParameter);
                }
            }
        }

        private string prettyPrintedConent;
        public string PrettyPrintedContent
        {
            get
            {
                return prettyPrintedConent;
            }
            set
            {
                prettyPrintedConent = value;
            }
        }

        private string documentation;
        public string Documentation
        {
            get
            {
                return documentation;
            }
            set
            {
                documentation = value;
            }
        }

        private ReadOnlyCollection<IParameter> parameters;
        public ReadOnlyCollection<IParameter> Parameters
        {
            get
            {
                return parameters;
            }
            set
            {
                parameters = value;
            }
        }

        public event EventHandler<CurrentParameterChangedEventArgs> CurrentParameterChanged;

        private ITextBuffer textBuffer;

        public Signature(ITextBuffer textBuffer, string content, string documentation, ReadOnlyCollection<IParameter> parameters)
        {
            this.textBuffer = textBuffer;
            this.content = content;
            this.documentation = documentation;
            this.parameters = parameters;

            this.textBuffer.Changed += textBuffer_Changed;

        }

        public void textBuffer_Changed(object sender, TextContentChangedEventArgs e)
        {
            ComputeCurrentParameter();
        }

        public void ComputeCurrentParameter()
        {
            if (parameters.Count == 0)
            {
                currentParameter = null;
                return;
            }

            string text = ApplicableToSpan.GetText(this.textBuffer.CurrentSnapshot);

            int currentIndex = 0;
            int commaCount = 0;
            while (currentIndex < text.Length)
            {
                int commaIndex = text.IndexOf(',', currentIndex);
                if (commaIndex == -1)
                {
                    break;
                }
                commaCount++;
                currentIndex = commaIndex + 1;
            }

            if (commaCount < Parameters.Count)
            {
                this.CurrentParameter = Parameters[commaCount];
            }
            else
            {
                //too many commas, use the last parameter as the current one.
                //
                this.CurrentParameter = Parameters[Parameters.Count - 1];
            }

        }

        private void RaiseParameterChanged(IParameter previous, IParameter current)
        {
            if (CurrentParameterChanged != null)
            {
                CurrentParameterChanged.Invoke(this, new CurrentParameterChangedEventArgs(previous, current));
            }
        }
    }


    #endregion

    #region Help Source and Provider

    internal class SignatureHelpSource : ISignatureHelpSource
    {
        private ITextBuffer textBuffer;
        private ITextStructureNavigator navigator;

        public SignatureHelpSource(ITextBuffer textBuffer, ITextStructureNavigator navigator)
        {
            this.textBuffer = textBuffer;
            this.navigator = navigator;
        }

        public void AugmentSignatureHelpSession(ISignatureHelpSession session, IList<ISignature> signatures)
        {
            ITextSnapshot snapshot = this.textBuffer.CurrentSnapshot;
            int position = session.GetTriggerPoint(this.textBuffer).GetPosition(snapshot);

            ITrackingSpan applicableToSpan = this.textBuffer.CurrentSnapshot.CreateTrackingSpan(
             new Span(position, 0), SpanTrackingMode.EdgeInclusive, 0);

            SnapshotPoint? sp = session.GetTriggerPoint(snapshot);
            if (!sp.HasValue)
            {
                return;
            }

            string word = navigator.GetExtentOfWord(sp.Value - 1).Span.GetText().Trim();
            ShaderlabDataManager.Instance.HLSLCGFunctions.ForEach(f =>
                {
                    foreach (var item in f.Synopsis)
                    {
                        if (f.Name.Equals(word))
                        {
                            signatures.Add(CreateSignature(this.textBuffer, item, f.Description, applicableToSpan));
                        }
                    }
                });

            ShaderlabDataManager.Instance.UnityBuiltinFunctions.ForEach(f =>
                {
                    foreach (var item in f.Synopsis)
                    {
                        if (f.Name.Equals(word))
                        {
                            signatures.Add(CreateSignature(this.textBuffer, item, f.Description, applicableToSpan));
                        }
                    }
                });
        }

        private Signature CreateSignature(ITextBuffer textBuffer, string sign, string documentation, ITrackingSpan span)
        {
            Signature signature = new Signature(textBuffer, sign, documentation, null);
            textBuffer.Changed += signature.textBuffer_Changed;

            //find the parameters in the method signature (expect methodname(one, two)
            string[] pars = sign.Split(new char[] { '(', ',', ')', ';' }, StringSplitOptions.RemoveEmptyEntries);
            List<IParameter> paramList = new List<IParameter>();

            int locusSearchStart = 0;
            for (int i = 1; i < pars.Length; i++)
            {
                string param = pars[i].Trim();

                if (string.IsNullOrEmpty(param))
                    continue;

                // find where this parameter is located in the method signature
                //
                int locusStart = sign.IndexOf(param, locusSearchStart);
                if (locusStart >= 0)
                {
                    Span locus = new Span(locusStart, param.Length);
                    locusSearchStart = locusStart + param.Length;
                    paramList.Add(new Parameter("", locus, param, signature));
                }
            }

            signature.Parameters = new ReadOnlyCollection<IParameter>(paramList);
            signature.ApplicableToSpan = span;
            signature.ComputeCurrentParameter();
            return signature;
        }

        public ISignature GetBestMatch(ISignatureHelpSession session)
        {
            return null;
        }

        private bool isDisposed;
        public void Dispose()
        {
            if (!isDisposed)
            {
                GC.SuppressFinalize(this);
                isDisposed = true;
            }
        }
    }

    [Export(typeof(ISignatureHelpSourceProvider))]
    [Name("SignatureHelpSourceProvider")]
    [Order(Before = "default")]
    [ContentType(Constants.ContentType)]
    internal class SignatureHelpSourceProvider : ISignatureHelpSourceProvider
    {
        [Import]
        public ITextStructureNavigatorSelectorService NagivatorService = null;

        public ISignatureHelpSource TryCreateSignatureHelpSource(ITextBuffer textBuffer)
        {
            return new SignatureHelpSource(textBuffer, NagivatorService.GetTextStructureNavigator(textBuffer));
        }
    }

    #endregion

    #region Command Handler and Provider

    internal class SignatureHelpCommand : IOleCommandTarget
    {
        IOleCommandTarget nextCommand;
        private ITextView textView;
        private ITextStructureNavigator navigator;
        private ISignatureHelpBroker broker;
        private ISignatureHelpSession session;

        public SignatureHelpCommand(IVsTextView textViewAdapter, ITextView textView, ITextStructureNavigator navigator, ISignatureHelpBroker broker)
        {
            this.textView = textView;
            this.navigator = navigator;
            this.broker = broker;

            textViewAdapter.AddCommandFilter(this, out nextCommand);
        }

        public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            char typedChar = char.MinValue;

            if (pguidCmdGroup == VSConstants.VSStd2K && nCmdID == (uint)VSConstants.VSStd2KCmdID.TYPECHAR)
            {
                typedChar = (char)(ushort)Marshal.GetObjectForNativeVariant(pvaIn);
                if (typedChar.Equals('('))
                {
                    //move the point back so it's in the preceding word
                    //
                    SnapshotPoint point = this.textView.Caret.Position.BufferPosition - 1;
                    TextExtent extent = this.navigator.GetExtentOfWord(point);
                    string word = extent.Span.GetText();

                    // if have on session dialog already, dismiss it.
                    // 
                    if (session != null)
                    {
                        session.Dismiss();
                    }

                    if (ShaderlabDataManager.Instance.UnityBuiltinFunctions.Any(f => f.Name.Equals(word, StringComparison.CurrentCultureIgnoreCase))
                        || ShaderlabDataManager.Instance.HLSLCGFunctions.Any(f => f.Name.Equals(word, StringComparison.CurrentCultureIgnoreCase)))
                    {
                        session = this.broker.TriggerSignatureHelp(this.textView);
                    }

                }
                else if (typedChar.Equals(')') && session != null)
                {
                    session.Dismiss();
                    session = null;
                }
            }
            return nextCommand.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
        }

        public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            return nextCommand.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
        }
    }

    [Export(typeof(IVsTextViewCreationListener))]
    [Name("SignatureHelpCommandProvider")]
    [TextViewRole(PredefinedTextViewRoles.Editable)]
    [ContentType(Constants.ContentType)]
    internal class SignatureHelpCommandProvider : IVsTextViewCreationListener
    {
        [Import]
        internal IVsEditorAdaptersFactoryService AdapterServides = null;

        [Import]
        internal ITextStructureNavigatorSelectorService NavigatorServices = null;

        [Import]
        internal ISignatureHelpBroker SignartureBroker = null;

        public void VsTextViewCreated(IVsTextView textViewAdapter)
        {
            ITextView textView = AdapterServides.GetWpfTextView(textViewAdapter);
            if (textView == null)
            {
                return;
            }

            textView.Properties.GetOrCreateSingletonProperty( () => new SignatureHelpCommand(textViewAdapter, textView, NavigatorServices.GetTextStructureNavigator(textView.TextBuffer), SignartureBroker));
        }
    }


    #endregion
}
