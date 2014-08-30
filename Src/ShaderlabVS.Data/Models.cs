using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaderlabVS.Data
{
    #region Common
    public class ModelBase
    {
        /// <summary>
        /// Do something to modify the property values
        /// </summary>
        public virtual void PrepareProperties()
        { }
    }

    public class FunctionBase : ModelBase
    {
        [DefinationKey("Name")]
        public string Name { get; set; }

        [DefinationKey("Synopsis")]
        public string AllSynopsisStr { get; set; }

        public List<string> SynopsisList { get; set; }

        [DefinationKey("Description")]
        public string Description { get; set; }

        public FunctionBase()
        {
            Name = string.Empty;
            AllSynopsisStr = string.Empty;
            SynopsisList = new List<string>();
            Description = string.Empty;
        }

        public override void PrepareProperties()
        {
            base.PrepareProperties();
            if (!string.IsNullOrEmpty(AllSynopsisStr.Trim()))
            {
                SynopsisList.Clear();
                SynopsisList.AddRange(AllSynopsisStr.Trim().Split(new char[] { ';', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).ToList());
                SynopsisList.ForEach(s => s = s.Trim());
            }
        }
    }

    #endregion

    #region HLSL/CG
    public class HLSLCGFunctions : FunctionBase
    {

    }

    public class HLSLCGKeywords : ModelBase
    {
        [DefinationKey("Type")]
        public string Type { get; set; }

        [DefinationKey("AllKeywords")]
        public string AllKeywords { get; set; }
       
        public List<string> KeywordsList { get; set; }

        public HLSLCGKeywords()
        {
            Type = string.Empty;
            AllKeywords = string.Empty;
            KeywordsList = new List<string>();
        }

        public override void PrepareProperties()
        {
            base.PrepareProperties();
            KeywordsList.Clear();
            HashSet<string> keywordSet = new HashSet<string>();
            var kwlist = AllKeywords.Split(new char[] { ',', ';', '\n', '\r', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            kwlist.ForEach(s => keywordSet.Add(s));
            KeywordsList.AddRange(keywordSet.ToList());
            KeywordsList.ForEach(s => s = s.Trim());
        }
    }

    #endregion

    #region Unity3D
    public class UnityBuiltinValue : ModelBase
    {
        [DefinationKey("Name")]
        public string Name { get; set; }

        [DefinationKey("Type")]
        public string Type { get; set; }

        [DefinationKey("Value")]
        public string VauleDescription { get; set; }

        public UnityBuiltinValue()
        {
            Name = string.Empty;
            Type = string.Empty;
            VauleDescription = string.Empty;
        }
    }

    public class UnityBuiltinFunction : FunctionBase
    {

    }


    public class UnityKeywords
    {
        [DefinationKey("Description")]
        public string Description { get; set; }

        [DefinationKey("Format")]
        public string Format { get; set; }

        public UnityKeywords()
            : base()
        {
            Description = string.Empty;
            Format = string.Empty;
        }
    }
    #endregion



}
