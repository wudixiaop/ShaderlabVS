using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace ShaderlabVS.Data
{
    /// <summary>
    /// Class use to read .def file
    /// 
    /// The .def syntax:
    ///     * Pair : 
    ///         - Description: Pair is something like KeyValuePair in C#, it has key and value defination.
    ///         - Format:  %$ (Key) = {(Value)}$%
    ///             - (Key) and (Value) are placeholders.
    ///             - Pair begin mark is "%$"
    ///             - Pair end mark is "$%"
    ///             - (Value) is in brace.
    ///             
    ///     * Section :
    ///         - Description: Section like a Dictionary in C#, it can be empty or has single or multiple Pair
    ///         - Format:  {% [Paris..] %}
    ///             - [Paris...] is placeholder for Pair
    ///             - Section begin mark is "{%"
    ///             - Section end mark is "%}"
    ///             - In same section, Pair with same key is not allowed
    ///             - A .def file can contains as many sections as you want
    ///             
    ///     * Comments :
    ///         - Description: lines are started with "#"
    /// 
    ///     * Escape Chars: 
    ///         chars  =>  escaped chars
    ///          {     =>   \{
    ///          }     =>   \}
    ///          $     =>   \$
    ///          =     =>   \=
    ///          %     =>   \%
    ///          #     =>   \#
    /// 
    /// </summary>
    public class DefinationReader
    {
        private List<Dictionary<string, string>> sections;

        /// <summary>
        /// Gets the Sections defined in the .def file 
        /// </summary>
        public List<Dictionary<string, string>> Sections
        {
            get
            {
                return sections;
            }
        }

        private string defFileName;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="defFileName">The .def file name</param>
        public DefinationReader(string defFileName)
        {
            sections = new List<Dictionary<string, string>>();
            this.defFileName = defFileName;
        }

        /// <summary>
        /// Read data from file
        /// </summary>
        public void Read()
        {
            if(!File.Exists(defFileName))
            {
                throw new FileNotFoundException(string.Format("{0} is not founded", defFileName));
            }

            string content = RemoveAllCommentsLines(File.ReadAllLines(defFileName)).Trim();
            string sectionPattern = @"\{%[\s\S]*?%\}";
            foreach (Match match in Regex.Matches(content, sectionPattern))
            {
                string m = match.ToString();
                sections.Add(ParseSectionFromeText(match.ToString()));
            }
        }

        private string RemoveAllCommentsLines(string[] lines)
        {
            StringBuilder newContent = new StringBuilder();
            foreach (string line in lines)
            {
                if (line.Trim().StartsWith("#"))
                {
                    continue;
                }

                newContent.AppendLine(line);
            }

            return newContent.ToString();
        }

        private Dictionary<string, string> ParseSectionFromeText(string sectionText)
        {
            Dictionary<string, string> sectionDict = new Dictionary<string, string>();
            string pairPattern = @"%\$(?<key>[\s\S]+?)=\s*?\{(?<value>[\s\S]*?)\}\s*?\$%";
            foreach (Match match in Regex.Matches(sectionText, pairPattern))
            {
                string key = match.Groups["key"].Value.ToString().Trim();
                string value = match.Groups["value"].Value.ToString().Trim();

                if (!string.IsNullOrEmpty(key))
                {
                    sectionDict.Add(Escape(key), Escape(value));
                }
            }

            return sectionDict;
        }

        private string Escape(string input)
        {
            return input.Replace(@"\#", "#")
                        .Replace(@"\{", "{")
                        .Replace(@"\}", "}")
                        .Replace(@"\$", "$")
                        .Replace(@"\=", "=")
                        .Replace(@"\%", "%");
        }
    }
}
