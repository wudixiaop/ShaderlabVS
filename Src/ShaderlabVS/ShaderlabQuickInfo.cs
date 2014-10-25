using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Utilities;
using ShaderlabVS.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace ShaderlabVS
{
    #region Shaderlab Quickinfo Source
    internal class ShaderlabQuickInfoSource : IQuickInfoSource
    {
        private ShaderlabQuickInfoSourceProvider provider;
        private ITextBuffer textBuffer;
        private Dictionary<string, string> quickInfos;

        public ShaderlabQuickInfoSource(ShaderlabQuickInfoSourceProvider provider, ITextBuffer textBuffer)
        {
            this.provider = provider;
            this.textBuffer = textBuffer;
            this.quickInfos = new Dictionary<string, string>();
            isDisposed = false;
            QuickInfoInit();
        }

        private void QuickInfoInit()
        {
            ShaderlabDataManager.Instance.HLSLCGFunctions.ForEach((f) =>
                {
                    if (quickInfos.ContainsKey(f.Name))
                    {
                        string info = quickInfos[f.Name];
                        info = info + string.Format("\nFunction: {0}", f.Description);
                        quickInfos[f.Name] = info;
                    }
                    else
                    {
                        quickInfos.Add(f.Name, f.Description);
                    }
                });

            ShaderlabDataManager.Instance.UnityBuiltinDatatypes.ForEach((d) =>
                {
                    if (quickInfos.ContainsKey(d.Name))
                    {
                        quickInfos[d.Name] = quickInfos[d.Name] + string.Format("\nUnity3d built-in balues: {0}", d.Description);
                    }
                    else
                    {
                        quickInfos.Add(d.Name, d.Description);
                    }
                });

            ShaderlabDataManager.Instance.UnityBuiltinFunctions.ForEach((f) =>
                {
                    if (quickInfos.ContainsKey(f.Name))
                    {
                        quickInfos[f.Name] = quickInfos[f.Name] + string.Format("\nUnity3D built-in function: {0}", f.Description);
                    }
                    else
                    {
                        quickInfos.Add(f.Name, f.Description);
                    }
                });

            ShaderlabDataManager.Instance.UnityBuiltinMacros.ForEach((f) =>
                {

                    string description = string.Format("{0}\n{1}", string.Join(";\n", f.Synopsis), f.Description);
                    if (quickInfos.ContainsKey(f.Name))
                    {
                        quickInfos[f.Name] = quickInfos[f.Name] + string.Format("\nUnity3D built-in macros: {0}", description);
                    }
                    else
                    {
                        quickInfos.Add(f.Name, description);
                    }
                });

            ShaderlabDataManager.Instance.UnityKeywords.ForEach((k) =>
                {
                    if (quickInfos.ContainsKey(k.Name))
                    {
                        quickInfos[k.Name] = quickInfos[k.Name] + string.Format("\nUnity3D keywords: {0}", k.Description);
                    }
                    else
                    {
                        quickInfos.Add(k.Name, k.Description);
                    }
                });

            ShaderlabDataManager.Instance.UnityBuiltinValues.ForEach((v) =>
                {
                    if (quickInfos.ContainsKey(v.Name))
                    {
                        quickInfos[v.Name] = quickInfos[v.Name] + string.Format("\nUnity3d built-in values: {0}", v.VauleDescription);
                    }
                    else
                    {
                        quickInfos.Add(v.Name, v.VauleDescription);
                    }
                });
        }

        public void AugmentQuickInfoSession(IQuickInfoSession session, IList<object> quickInfoContent, out Microsoft.VisualStudio.Text.ITrackingSpan applicableToSpan)
        {
            SnapshotPoint? sp = session.GetTriggerPoint(this.textBuffer.CurrentSnapshot);
            if (!sp.HasValue)
            {
                applicableToSpan = null;
                return;
            }

            ITextSnapshot currentSnapshot = sp.Value.Snapshot;
            SnapshotSpan span = new SnapshotSpan(sp.Value, 0);

            ITextStructureNavigator navigator = provider.NavigatorService.GetTextStructureNavigator(this.textBuffer);
            string keyText = navigator.GetExtentOfWord(sp.Value).Span.GetText().Trim();

            if (string.IsNullOrEmpty(keyText))
            {
                applicableToSpan = null;
                return;
            }

            string info;
            quickInfos.TryGetValue(keyText, out info);
            if (!string.IsNullOrEmpty(info))
            {
                applicableToSpan = currentSnapshot.CreateTrackingSpan(span.Start.Position, 9, SpanTrackingMode.EdgeInclusive);
                quickInfoContent.Add(info);
                return;
            }

            applicableToSpan = null;
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

    #endregion

    #region Shaderlab Quickinfo Source Provider
    [Export(typeof(IQuickInfoSourceProvider))]
    [Name("ShaderlabQuickInfoSourceProvider")]
    [Order(Before = "Default Quick Info Presenter")]
    [ContentType(Constants.ContentType)]
    internal class ShaderlabQuickInfoSourceProvider : IQuickInfoSourceProvider
    {
        [Import]
        public ITextStructureNavigatorSelectorService NavigatorService = null;

        [Import]
        public ITextBufferFactoryService TextBufferFactoryService = null;

        public IQuickInfoSource TryCreateQuickInfoSource(ITextBuffer textBuffer)
        {
            return new ShaderlabQuickInfoSource(this, textBuffer);
        }
    }
    #endregion

    #region Shaderlab Quickinfo Controller

    internal class ShaderlabQuickInfoController : IIntellisenseController
    {
        private ITextView textView;
        private IList<ITextBuffer> textBuffers;
        private ShaderlabQuickInfoControllerProvider controllerProvider;
        private IQuickInfoSession quickInfoSession;

        public ShaderlabQuickInfoController(ITextView textView, IList<ITextBuffer> textBuffers, ShaderlabQuickInfoControllerProvider controllerProvider)
        {
            this.textBuffers = textBuffers;
            this.textView = textView;
            this.controllerProvider = controllerProvider;

            textView.MouseHover += textView_MouseHover;
        }

        void textView_MouseHover(object sender, MouseHoverEventArgs e)
        {
            SnapshotPoint? ssPoint = textView.BufferGraph.MapDownToFirstMatch(new SnapshotPoint(textView.TextSnapshot, e.Position),
                                                                            PointTrackingMode.Positive,
                                                                            snapshot => textBuffers.Contains(snapshot.TextBuffer),
                                                                            PositionAffinity.Predecessor);
            if (ssPoint != null)
            {
                ITrackingPoint point = ssPoint.Value.Snapshot.CreateTrackingPoint(ssPoint.Value.Position, PointTrackingMode.Positive);

                if (!controllerProvider.QuickInfoBroker.IsQuickInfoActive(this.textView))
                {
                    quickInfoSession = controllerProvider.QuickInfoBroker.TriggerQuickInfo(textView, point, true);
                }
            }
        }

        public void Detach(Microsoft.VisualStudio.Text.Editor.ITextView textView)
        {
            if (this.textView == textView)
            {
                this.textView.MouseHover -= textView_MouseHover;
                this.textView = null;
            }
        }

        public void DisconnectSubjectBuffer(ITextBuffer subjectBuffer)
        {

        }

        public void ConnectSubjectBuffer(ITextBuffer subjectBuffer)
        {

        }
    }

    #endregion

    #region Shaderlab Quickinfo Controller Provoider

    [Export(typeof(IIntellisenseControllerProvider))]
    [Name("ShaderlabQuickInfoControllerProvider")]
    [ContentType(Constants.ContentType)]
    internal class ShaderlabQuickInfoControllerProvider : IIntellisenseControllerProvider
    {
        [Import]
        public IQuickInfoBroker QuickInfoBroker { get; set; }


        public IIntellisenseController TryCreateIntellisenseController(ITextView textView, IList<ITextBuffer> subjectBuffers)
        {
            return new ShaderlabQuickInfoController(textView, subjectBuffers, this);
        }
    }

    #endregion
}
