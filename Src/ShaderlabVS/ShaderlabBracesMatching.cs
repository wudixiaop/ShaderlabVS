using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace ShaderlabVS
{
    internal class ShaderlabBracesTagger : ITagger<TextMarkerTag>
    {
        private ITextView textView;
        private ITextBuffer textBuffer;
        private Dictionary<char, char> braces;
        private SnapshotPoint? currentCharSnapPoint;

        public ShaderlabBracesTagger(ITextView textView, ITextBuffer buffer)
        {
            this.textView = textView;
            this.textBuffer = buffer;

            braces = new Dictionary<char, char>()
            {
                {'{', '}'},
                {'(', ')'},
                {'[', ']'}
            };

            this.currentCharSnapPoint = null;

            this.textView.Caret.PositionChanged += Caret_PositionChanged;
            this.textView.LayoutChanged += textView_LayoutChanged;
        }

        private void textView_LayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            if (e.NewSnapshot != e.OldSnapshot)
            {
                UpdateAtCaretPosition(this.textView.Caret.Position);
            }
        }

        private void Caret_PositionChanged(object sender, CaretPositionChangedEventArgs e)
        {
            UpdateAtCaretPosition(e.NewPosition);
        }

        private void UpdateAtCaretPosition(CaretPosition caretPos)
        {
            currentCharSnapPoint = caretPos.Point.GetPoint(this.textBuffer, caretPos.Affinity);

            if (currentCharSnapPoint.HasValue)
            {
                if (TagsChanged != null)
                {
                    ITextSnapshot currentSnapshot = this.textBuffer.CurrentSnapshot;
                    TagsChanged.Invoke(this, new SnapshotSpanEventArgs(new SnapshotSpan(currentSnapshot, 0, currentSnapshot.Length)));
                }
            }
        }

        public IEnumerable<ITagSpan<TextMarkerTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            if (spans.Count == 0 
                || currentCharSnapPoint == null 
                || currentCharSnapPoint.Value.Snapshot.Length <= currentCharSnapPoint.Value.Position)
            {
                yield break;
            }
            
           
            // if in commandline, we do nothing
            //
            if (Utilities.IsInCommentLine(currentCharSnapPoint.Value))
            {
                yield break;
            }

            // For open char, the matched state trigger when the caret at the right side of the brace char.
            // For closed char, the matched state trigger when the caret at right side of the brace char.
            // 
            char currentChar = currentCharSnapPoint.Value.GetChar();
            SnapshotPoint? lastCharSnapShot = currentCharSnapPoint == 0 ? currentCharSnapPoint : currentCharSnapPoint - 1; 
            char lastChar = lastCharSnapShot.Value.GetChar();

            SnapshotPoint? matchedPosition = null;

            if (IsOpenBrace(currentChar))
            {
                char closedChar = this.GetClosedCharByOpenBrace(currentChar);
                FindMatchingBrace(currentCharSnapPoint.Value, currentChar, closedChar, true, ref matchedPosition);

                if (matchedPosition.HasValue)
                {
                    yield return new TagSpan<TextMarkerTag>(new SnapshotSpan(currentCharSnapPoint.Value, 1), new TextMarkerTag(Constants.ShaderlabBracesMarker));
                    yield return new TagSpan<TextMarkerTag>(new SnapshotSpan(matchedPosition.Value, 1), new TextMarkerTag(Constants.ShaderlabBracesMarker));
                }
            }

            if(IsCloseBrace(lastChar))
            {
                char openChar = this.GetOpenCharByClosedBrace(lastChar);
                FindMatchingBrace(currentCharSnapPoint.Value, openChar, lastChar, false, ref matchedPosition);

                if (matchedPosition.HasValue)
                {
                    yield return new TagSpan<TextMarkerTag>(new SnapshotSpan(lastCharSnapShot.Value, 1), new TextMarkerTag(Constants.ShaderlabBracesMarker));
                    yield return new TagSpan<TextMarkerTag>(new SnapshotSpan(matchedPosition.Value, 1), new TextMarkerTag(Constants.ShaderlabBracesMarker));
                }
            }

        }

        private void FindMatchingBrace(SnapshotPoint startPoint, char openChar, char closedChar, bool IsMatchingClosedChar, ref SnapshotPoint? matchedPosition)
        {
            SnapshotPoint currentCheckPos = startPoint - 1;
            int step = -1;

            if (IsMatchingClosedChar)
            {
                currentCheckPos = startPoint;
                step = 1;
            }

            int matchIndex = 1;

            while (currentCheckPos > 0 && currentCheckPos.Position <= currentCheckPos.Snapshot.Length - 1)
            {
                currentCheckPos += step;

                if (currentCheckPos < 0)
                {
                    break;
                }

                char currentCheckChar = currentCheckPos.GetChar();

                if (IsMatchingClosedChar)
                {
                    if (currentCheckChar == openChar && !Utilities.IsInCommentLine(currentCheckPos))
                    {
                        matchIndex++;
                    }

                    if (currentCheckChar == closedChar && !Utilities.IsInCommentLine(currentCheckPos))
                    {
                        matchIndex--;
                    }
                }
                else
                {
                    if (currentCheckChar == closedChar && !Utilities.IsInCommentLine(currentCheckPos))
                    {
                        matchIndex++;
                    }

                    if (currentCheckChar == openChar && !Utilities.IsInCommentLine(currentCheckPos))
                    {
                        matchIndex--;
                    }
                }

                // if match index equals to 0, we think we find the matched brace char out.
                //
                if (matchIndex == 0)
                {
                    break;
                }
            }

            if (matchIndex == 0)
            {
                matchedPosition = currentCheckPos;
                return;
            }
        }

        private bool IsOpenBrace(char c)
        {
            return this.braces.ContainsKey(c);
        }

        private bool IsCloseBrace(char c)
        {
            return this.braces.ContainsValue(c);
        }

        private char GetOpenCharByClosedBrace(char closedChar)
        {
            return this.braces.Where(b => b.Value.Equals(closedChar)).First().Key;
        }

        private char GetClosedCharByOpenBrace(char openChar)
        {
            char closedChar;
            this.braces.TryGetValue(openChar, out closedChar);
            return closedChar;
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
    }

    [Export(typeof(IViewTaggerProvider))]
    [ContentType(Constants.ContentType)]
    [TagType(typeof(TextMarkerTag))]
    internal class ShaderlabBraceMatchingTaggerProvider : IViewTaggerProvider
    {
        public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag
        {
            if (textView == null || buffer == null)
            {
                return null;
            }

            return (new ShaderlabBracesTagger(textView, buffer)) as ITagger<T>;

        }
    }
}
