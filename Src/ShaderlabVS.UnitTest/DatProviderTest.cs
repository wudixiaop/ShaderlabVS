using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShaderlabVS.Data;

namespace ShaderlabVS.UnitTest
{
    [TestClass]
    public class DatProviderTest
    {
        [TestMethod]
        public void TestUnityBuiltInValues()
        {
            string defFileName = "Unity3D_values.def";
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
            string file = "HLSL_CG_functions.def";
            var funList = DefinationDataProvider<FunctionBase>.ProvideFromFile(file);
            foreach (FunctionBase fun in funList)
            {
                Console.WriteLine("-----------------Function--------------------");
                Console.WriteLine("Name={0}", fun.Name);
                Console.WriteLine("Synopsis count={0}", fun.SynopsisList.Count);
                Console.WriteLine("Synopsis = {0}", string.Join("\n", fun.SynopsisList));
                Console.WriteLine("Description = {0}", string.Join("\n", fun.Description));
            }
        }

        [TestMethod]
        public void TestHLSLKeywords()
        {
            string file = "HLSL_CG_Keywords.def";
            var funList = DefinationDataProvider<HLSLCGKeywords>.ProvideFromFile(file);
            foreach (HLSLCGKeywords fun in funList)
            {
                Console.WriteLine("-----------------Function--------------------");
                Console.WriteLine("Name={0}", fun.Type);
                Console.WriteLine("Keywords = {0}", string.Join("\n", fun.KeywordsList));
            }
        }
    }
}
