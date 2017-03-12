using Microsoft.VisualStudio.Text;
using System;

namespace ShaderlabVS
{
    internal class Utilities
    {
        public static bool IsCommentLine(string lineText)
        {
            string checkText = lineText.Trim();
            if (checkText.StartsWith("//")
               || checkText.StartsWith("/*")
               || checkText.EndsWith("*/"))
            {
                return true;
            }

            return false;
        }

        public static bool IsInCommentLine(SnapshotPoint position)
        {
            string lineText = position.GetContainingLine().GetText();
            return Utilities.IsCommentLine(lineText);
        }

        public static int IndexOfNonWhitespaceCharacter(string text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                if (!Char.IsWhiteSpace(text[i]))
                {
                    return i;
                }
            }
            return -1;
        }

        public static bool IsInCGOrHLSLFile(string filePath)
        {
            var lower = filePath.ToLower();
            return lower.EndsWith(".cg") || lower.EndsWith(".hlsl");
        }
    }
}
