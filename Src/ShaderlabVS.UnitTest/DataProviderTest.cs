using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShaderlabVS.Data;
using System;

namespace ShaderlabVS.UnitTest
{
    [TestClass]
    public class DataProviderTest
    {
        [TestMethod]
        public void TestUnityBuiltInValues()
        {
            string defFileName = "Data\\Unity3D_values.def";
            DefinationReader dr = new DefinationReader(defFileName);
            dr.Read();
            Console.WriteLine(dr.Sections.Count);

            var dataList = DefinationDataProvider<UnityBuiltinValue>.ProvideFromFile(defFileName);
            Console.WriteLine(dataList.Count);

            foreach (var val in dataList)
            {
                Console.WriteLine("-----------------Values--------------------");
                Console.WriteLine("Name={0}", val.Name);
                Console.WriteLine("Type={0}", val.Type);
                Console.WriteLine("Description={0}", val.VauleDescription);
            }

        }

        [TestMethod]
        public void TestHLSLCGFunctions()
        {
            string file = "Data\\HLSL_CG_functions.def";
            var funList = DefinationDataProvider<HLSLCGFunction>.ProvideFromFile(file);
            foreach (HLSLCGFunction fun in funList)
            {
                Console.WriteLine("-----------------Function--------------------");
                Console.WriteLine("Name={0}", fun.Name);
                Console.WriteLine("Synopsis count={0}", fun.Synopsis.Count);
                Console.WriteLine("Synopsis = {0}", string.Join("\n", fun.Synopsis));
                Console.WriteLine("Description = {0}", string.Join("\n", fun.Description));
            }
        }

        [TestMethod]
        public void TestHLSLKeywords()
        {
            string file = "Data\\HLSL_CG_Keywords.def";
            var funList = DefinationDataProvider<HLSLCGKeywords>.ProvideFromFile(file);
            foreach (HLSLCGKeywords fun in funList)
            {
                Console.WriteLine("-----------------Function--------------------");
                Console.WriteLine("Name={0}", fun.Type);
                Console.WriteLine("Keywords = {0}", string.Join("\n", fun.Keywords));
            }
        }

        [TestMethod]
        public void TestHLSLDatatype()
        {
            string file = "Data\\HLSL_CG_datatype.def";
            var list = DefinationDataProvider<HLSLCGDataTypes>.ProvideFromFile(file);
            foreach (HLSLCGDataTypes dt in list)
            {
                Console.WriteLine("-----------------datatype--------------------");
                Console.WriteLine("datatype = {0}", string.Join("\n", dt.DataTypes));
            }
        }

        [TestMethod]
        public void TestUnitydatatype()
        {
            string file = "Data\\Unity3D_datatype.def";
            var list = DefinationDataProvider<UnityBuiltinDatatype>.ProvideFromFile(file);
            foreach (var item in list)
            {
                Console.WriteLine("-----------------datatype--------------------");
                Console.WriteLine("Name = {0}", string.Join("\n", item.Name));
                Console.WriteLine("Description = {0}", string.Join("\n", item.Description));
            }
        }

        [TestMethod]
        public void TestUnityFunctions()
        {
            string file = "Data\\Unity3D_functions.def";
            var funList = DefinationDataProvider<UnityBuiltinFunction>.ProvideFromFile(file);
            foreach (UnityBuiltinFunction fun in funList)
            {
                Console.WriteLine("-----------------Function--------------------");
                Console.WriteLine("Name={0}", fun.Name);
                Console.WriteLine("Synopsis count={0}", fun.Synopsis.Count);
                Console.WriteLine("Synopsis = {0}", string.Join("\n", fun.Synopsis));
                Console.WriteLine("Description = {0}", string.Join("\n", fun.Description));
            }
        }

        [TestMethod]
        public void TestUnityMacros()
        {
            string file = "Data\\Unity3D_macros.def";
            var list = DefinationDataProvider<UnityBuiltinMacros>.ProvideFromFile(file);
            foreach (var item in list)
            {
                Console.WriteLine("-----------------Macros--------------------");
                Console.WriteLine("Name={0}", item.Name);
                Console.WriteLine("Synopsis count={0}", item.Synopsis.Count);
                Console.WriteLine("Synopsis = {0}", string.Join("\n", item.Synopsis));
                Console.WriteLine("Description = {0}", string.Join("\n", item.Description));
            }
        }

        [TestMethod]
        public void TestDataManger()
        {
            Console.WriteLine(string.Join("\r", ShaderlabDataManager.Instance.HLSLCGBlockKeywords));
        }
    }
}
