using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;
using System;
using System.ComponentModel.Composition;

namespace ShaderlabVS
{
    #region Shortcuts Register

    internal class ShaderlabShortcuts : IOleCommandTarget
    {
        private IOleCommandTarget nextTarget;
        private ITextView textView;

        public ShaderlabShortcuts(IVsTextView textViewAdatper, ITextView textView)
        {
            this.textView = textView;

            textViewAdatper.AddCommandFilter(this, out nextTarget);
        }

        public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            if (pguidCmdGroup == Constants.VSStd2KcmdGuid)
            {
                switch ((VSConstants.VSStd2KCmdID)nCmdID)
                {
                    case VSConstants.VSStd2KCmdID.COMMENTBLOCK:
                    case VSConstants.VSStd2KCmdID.COMMENT_BLOCK:
                        if (CommentsHelper.CommentOrUnCommentBlock(this.textView, true))
                        {
                            return VSConstants.S_OK;
                        }
                        break;

                    case VSConstants.VSStd2KCmdID.UNCOMMENTBLOCK:
                    case VSConstants.VSStd2KCmdID.UNCOMMENT_BLOCK:
                        if (CommentsHelper.CommentOrUnCommentBlock(this.textView, false))
                        {
                            return VSConstants.S_OK;
                        }
                        break;
                    default:
                        break;
                }
            }
            else if (pguidCmdGroup == Constants.VS97CmdGuid)
            {
                switch ((VSConstants.VSStd97CmdID)nCmdID)
                {
                    case VSConstants.VSStd97CmdID.F1Help:
                        break;
                    default:
                        break;
                }
            }

            

            return nextTarget.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
        }

        public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            // Command Filter
            //
            if (pguidCmdGroup == Constants.VSStd2KcmdGuid)
            {
                for (int i = 0; i < cCmds; i++)
                {
                    switch ((VSConstants.VSStd2KCmdID)prgCmds[i].cmdID)
                    {
                        case VSConstants.VSStd2KCmdID.FORMATDOCUMENT:
                        case VSConstants.VSStd2KCmdID.FORMATSELECTION:
                        case VSConstants.VSStd2KCmdID.COMMENT_BLOCK:
                        case VSConstants.VSStd2KCmdID.COMMENTBLOCK:
                        case VSConstants.VSStd2KCmdID.UNCOMMENT_BLOCK:
                        case VSConstants.VSStd2KCmdID.UNCOMMENTBLOCK:
                        case VSConstants.VSStd2KCmdID.HELP:
                            prgCmds[i].cmdf = (uint)(OLECMDF.OLECMDF_ENABLED | OLECMDF.OLECMDF_SUPPORTED);
                            return VSConstants.S_OK;
                    }
                }
            }
            else if (pguidCmdGroup == Constants.VS97CmdGuid)
            {
                for (int i = 0; i < cCmds; i++)
                {
                    switch ((VSConstants.VSStd97CmdID)prgCmds[i].cmdID)
                    {
                        case VSConstants.VSStd97CmdID.F1Help:
                            prgCmds[i].cmdf = (uint)(OLECMDF.OLECMDF_ENABLED | OLECMDF.OLECMDF_SUPPORTED);
                            return VSConstants.S_OK;
                        default:
                            break;
                    }
                }
            }

            return nextTarget.QueryStatus(pguidCmdGroup, cCmds, prgCmds, pCmdText);
        }
    }

    [Export(typeof(IVsTextViewCreationListener))]
    [ContentType(Constants.ContentType)]
    [Name(Constants.ShaderlabShortcuts)]
    [TextViewRole(PredefinedTextViewRoles.Editable)]
    internal class ShortcutBindingProvider : IVsTextViewCreationListener
    {

        [Import(typeof(IVsEditorAdaptersFactoryService))]
        internal IVsEditorAdaptersFactoryService AdapterServices = null;

        public void VsTextViewCreated(IVsTextView textViewAdapter)
        {
            IWpfTextView textView = AdapterServices.GetWpfTextView(textViewAdapter);
            if (textView == null)
            {
                return;
            }

            textView.Properties.GetOrCreateSingletonProperty<ShaderlabShortcuts>(() =>
                {

                    return new ShaderlabShortcuts(textViewAdapter, textView);
                });
        }
    }

    #endregion

    #region Comments Feature

    internal class CommentsHelper
    {
        public static bool CommentOrUnCommentBlock(ITextView textView, bool comment)
        {
            // Vars for prevoius states before Map action
            //
            SnapshotPoint startPoint;
            SnapshotPoint endPoint;

            // Vars for states after Map action
            //
            SnapshotPoint? start;
            SnapshotPoint? end;

            if (textView.Selection.IsActive && !textView.Selection.IsEmpty)
            {
                startPoint = textView.Selection.Start.Position;
                endPoint = textView.Selection.End.Position;

                start = textView.BufferGraph.MapDownToFirstMatch(startPoint,
                                                                     PointTrackingMode.Positive,
                                                                     IsShaderlabContent,
                                                                     PositionAffinity.Successor);
                var endLine = startPoint.GetContainingLine();
                if (endLine.Start == endPoint)
                {
                    endPoint = endPoint.Snapshot.GetLineFromLineNumber(endLine.LineNumber - 1).End;
                }

                end = textView.BufferGraph.MapDownToFirstMatch(endPoint,
                                                                    PointTrackingMode.Positive,
                                                                    IsShaderlabContent,
                                                                    PositionAffinity.Successor);

            }
            else
            {
                // one line
                //
                startPoint = textView.Caret.Position.BufferPosition;
                endPoint = startPoint;

                start = textView.BufferGraph.MapDownToFirstMatch(startPoint,
                                                                    PointTrackingMode.Positive,
                                                                    IsShaderlabContent,
                                                                    PositionAffinity.Successor);
                end = start;
            }

            if (start.HasValue && end.HasValue)
            {
                using (var editor = start.Value.Snapshot.TextBuffer.CreateEdit())
                {
                    int startLineNumber = start.Value.GetContainingLine().LineNumber;
                    int endLineNumber = end.Value.GetContainingLine().LineNumber;

                    if (comment)
                    {
                        // Do comment Action
                        //
                        int min = Int32.MaxValue;

                        // get the mini postion to place the comment
                        //
                        for (int i = startLineNumber; i <= endLineNumber; i++)
                        {
                            string lineText = start.Value.Snapshot.GetLineFromLineNumber(i).GetText();
                            int firstOfNonWhiteIndex = Utilities.IndexOfNonWhitespaceCharacter(lineText);
                            if (firstOfNonWhiteIndex >= 0 && firstOfNonWhiteIndex < min)
                            {
                                min = firstOfNonWhiteIndex;
                            }
                        }

                        for (int i = startLineNumber; i <= endLineNumber; i++)
                        {
                            var line = start.Value.Snapshot.GetLineFromLineNumber(i);
                            string lineText = line.GetText();
                            if (string.IsNullOrWhiteSpace(lineText))
                            {
                                continue;
                            }

                            editor.Insert(line.Start.Position + min, "//");
                        }
                    }
                    else
                    {
                        // Do uncomment action
                        //
                        for (int i = startLineNumber; i <= endLineNumber; i++)
                        {
                            var line = start.Value.Snapshot.GetLineFromLineNumber(i);
                            string lineText = line.GetText();
                            string lineTextAfterTrim = lineText.Trim();
                            if (lineTextAfterTrim.StartsWith("//"))
                            {
                                int commentIndex = lineText.IndexOf("//");

                                if (commentIndex != -1)
                                {
                                    editor.Delete(line.Start.Position + commentIndex, 2);
                                }
                            }
                            else
                            {
                                if (lineTextAfterTrim.EndsWith("*/"))
                                {
                                    int endCommentIndex = lineText.IndexOf("*/");
                                    editor.Delete(line.Start.Position + endCommentIndex, 2);
                                }

                                if (lineTextAfterTrim.StartsWith("/*"))
                                {
                                    int startCommentIndex = lineText.IndexOf("/*");
                                    editor.Delete(line.Start.Position + startCommentIndex, 2);
                                }

                                if (startLineNumber != endLineNumber && lineTextAfterTrim.StartsWith("*"))
                                {
                                    int startCommentIndex = lineText.IndexOf("*");
                                    editor.Delete(line.Start.Position + startCommentIndex, 1);
                                }
                            }
                        }
                    }

                    // Apply the modification
                    //
                    editor.Apply();
                }

                return true;
            }

            return false;
        }

        private static bool IsShaderlabContent(ITextSnapshot snap)
        {
            return snap.ContentType.IsOfType(Constants.ContentType);
        }
    }

    #endregion
}
