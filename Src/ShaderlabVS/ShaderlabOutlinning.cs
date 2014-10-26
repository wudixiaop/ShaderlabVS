using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;

namespace ShaderlabVS
{
    internal class Region {
        public int Start { get; set; }
        public int End { get; set; }
    }

    internal class ShaderlabOutlining : ITagger<OutliningRegionTag>
    {
        private ITextBuffer textBuffer;
        private List<Region> regions;

        public ShaderlabOutlining(ITextBuffer buffer)
        {
            this.textBuffer = buffer;
            this.regions = new List<Region>();
            this.textBuffer.Changed += textBuffer_Changed;
            GetRegions();
        }

        void textBuffer_Changed(object sender, TextContentChangedEventArgs e)
        {
            if (e.After == textBuffer.CurrentSnapshot)
            {
                GetRegions();
            }
        }

        private void GetRegions()
        {
            ITextSnapshot textSnapShot = this.textBuffer.CurrentSnapshot;
            List<Region> tempRegions = new List<Region>();

            foreach (var line in textSnapShot.Lines)
            {
                string lineText = line.GetText();
                int startIndex = lineText.IndexOf('{');

                // if we see start region symbol in non-comment line
                //
                if (!Utilities.IsCommentLine(lineText) && startIndex != -1)
                {
                    Region r = new Region();
                    r.Start = line.LineNumber;

                    SnapshotPoint? matchedPoint = FindMatchEndRegion(line.Start + startIndex);
                    if (matchedPoint.HasValue)
                    {
                        r.End = matchedPoint.Value.GetContainingLine().LineNumber;
                        tempRegions.Add(r);
                    }
                }
            }

            this.regions.Clear();
            this.regions.AddRange(tempRegions);

            if (TagsChanged != null)
            {
                TagsChanged.Invoke(this, new SnapshotSpanEventArgs(new SnapshotSpan(textSnapShot, 0, textSnapShot.Length)));
            }
        }

        private SnapshotPoint? FindMatchEndRegion(SnapshotPoint start)
        {
            SnapshotPoint checkPos = start;
            int matchIndex = 1;

            while (checkPos.Position < start.Snapshot.Length - 1)
            {
                checkPos += 1;
                char checkText = checkPos.GetChar();

                if (checkText == '{' && !Utilities.IsInCommentLine(checkPos))
                {
                    matchIndex++;
                }

                if (checkText == '}' && !Utilities.IsInCommentLine(checkPos))
                {
                    matchIndex--;
                }

                if (matchIndex == 0)
                {
                    break;
                }
            }

            if (matchIndex == 0)
            {
                return checkPos;
            }

            return null;
        }

        public IEnumerable<ITagSpan<OutliningRegionTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            if (spans.Count == 0)
            {
                yield break;
            }

            List<Region> currentRegions = this.regions;
            SnapshotSpan entire = new SnapshotSpan(this.textBuffer.CurrentSnapshot, 0, this.textBuffer.CurrentSnapshot.Length);
            int startLineNumber = entire.Start.GetContainingLine().LineNumber;
            int endLineNumber = entire.End.GetContainingLine().LineNumber;

            foreach (Region region in currentRegions)
            {
                if (region.Start >= startLineNumber && region.End <= endLineNumber)
                {
                    // We need to determine where the start point is. there are some scenarios:
                    //    1. the '{' is not in a new line, but it's at the end of a code line
                    //    2. the '{' at the begin of a line (if we ingore the white chars)
                    //
                    var line = entire.Snapshot.GetLineFromLineNumber(region.Start);
                    string lineText = line.GetText();

                    int offset = lineText.LastIndexOf("{");

                    // let's assume the default is that '{' is the #1 scenario
                    //
                    SnapshotPoint regionStart = entire.Snapshot.GetLineFromLineNumber(region.Start).Start + offset;

                    // if in #2 scenario
                    //
                    if (region.Start >= 0 && (lineText.Trim().Equals("{") || lineText.Trim().StartsWith("{")))
                    {
                        regionStart = entire.Snapshot.GetLineFromLineNumber(region.Start - 1).End;
                    }

                    SnapshotPoint regionEnd = entire.Snapshot.GetLineFromLineNumber(region.End).End;

                    StringBuilder hoverText = new StringBuilder();
                    int nonWhiteIndex = Utilities.IndexOfNonWhitespaceCharacter(lineText);

                    for (int i = regionStart.GetContainingLine().LineNumber; i <= regionEnd.GetContainingLine().LineNumber; )
                    {
                       
                        string hoverTextLine = entire.Snapshot.GetLineFromLineNumber(i).GetText();

                        if(Utilities.IndexOfNonWhitespaceCharacter(hoverTextLine) >= nonWhiteIndex)
                        { 
                            // Remove some the white chars
                            //
                            hoverText.AppendLine(hoverTextLine.Substring(nonWhiteIndex));
                        }
                        else
                        {
                            hoverText.AppendLine(hoverTextLine);
                        }

                        if (i - regionStart.GetContainingLine().LineNumber >= 8)
                        {
                            hoverText.AppendLine("...");
                            break;
                        }

                        i++;
                    }

                    OutliningRegionTag tag = new OutliningRegionTag(false, false, "...", hoverText.ToString());

                    yield return new TagSpan<OutliningRegionTag>(new SnapshotSpan(regionStart, regionEnd), tag);
                }
            }

        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
    }

    [Export(typeof(ITaggerProvider))]
    [TagType(typeof(OutliningRegionTag))]
    [ContentType(Constants.ContentType)]
    internal class ShaderlabOutliningTagProvider : ITaggerProvider
    {
        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            return buffer.Properties.GetOrCreateSingletonProperty(() =>
                    {
                        return (new ShaderlabOutlining(buffer)) as ITagger<T>;
                    });
        }
    }

}
