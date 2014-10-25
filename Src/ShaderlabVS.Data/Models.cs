using System;
using System.Collections.Generic;
using System.Linq;

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
        public string RawSynopsisData { get; set; }

        public List<string> Synopsis { get; set; }

        [DefinationKey("Description")]
        public string Description { get; set; }

        public FunctionBase()
        {
            Name = string.Empty;
            RawSynopsisData = string.Empty;
            Synopsis = new List<string>();
            Description = string.Empty;
        }

        public override void PrepareProperties()
        {
            base.PrepareProperties();
            if (!string.IsNullOrEmpty(RawSynopsisData.Trim()))
            {
                Synopsis.Clear();
                Synopsis.AddRange(RawSynopsisData.Trim().Split(new char[] { ';', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).ToList());
                Synopsis.ForEach(s => s = s.Trim());
            }
        }
    }

    #endregion

    #region HLSL/CG
    public class HLSLCGFunction : FunctionBase
    {

    }

    public class HLSLCGKeywords : ModelBase
    {
        [DefinationKey("Type")]
        public string Type { get; set; }

        [DefinationKey("AllKeywords")]
        public string RawKeywordsData { get; set; }

        public List<string> Keywords { get; set; }

        public HLSLCGKeywords()
        {
            Type = string.Empty;
            RawKeywordsData = string.Empty;
            Keywords = new List<string>();
        }

        public override void PrepareProperties()
        {
            base.PrepareProperties();
            Keywords.Clear();
            HashSet<string> keywordSet = new HashSet<string>();
            var kwlist = RawKeywordsData.Split(new char[] { ',', ';', '\n', '\r', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            kwlist.ForEach(s => keywordSet.Add(s));
            Keywords.AddRange(keywordSet.ToList());
            Keywords.ForEach(s => s = s.Trim());
        }
    }

    public class HLSLCGDataTypes : ModelBase
    {
        [DefinationKey("Alltypes")]
        public string RawDataTypeData { get; set; }

        public List<string> DataTypes { get; set; }

        public HLSLCGDataTypes()
        {
            RawDataTypeData = string.Empty;
            DataTypes = new List<string>();
        }

        public override void PrepareProperties()
        {
            base.PrepareProperties();
            DataTypes.Clear();
            HashSet<String> datatypes = new HashSet<string>();
            var list = RawDataTypeData.Split(new char[] { ',', ';', '\n', '\r', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            list.ForEach(s => datatypes.Add(s));
            DataTypes.AddRange(datatypes.ToList());
            DataTypes.ForEach(s => s = s.Trim());
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

    public class UnityBuiltinMacros : FunctionBase
    {

    }


    public class UnityKeywords : ModelBase
    {
        [DefinationKey("Name")]
        public string Name { get; set; }

        [DefinationKey("Description")]
        public string Description { get; set; }

        [DefinationKey("Format")]
        public string Format { get; set; }

        public UnityKeywords()
        {
            Description = string.Empty;
            Format = string.Empty;
        }
    }

    public class UnityBuiltinDatatype : ModelBase
    {
        [DefinationKey("Name")]
        public string Name { get; set; }

        [DefinationKey("Description")]
        public string Description { get; set; }

        public UnityBuiltinDatatype()
        {
            Name = string.Empty;
            Description = string.Empty;
        }
    }

    #endregion
}
