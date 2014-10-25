using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShaderlabVS.Data;
using System;
using System.Text;

namespace ShaderlabVS.UnitTest
{
    [TestClass]
    public class DefinationReaderTest
    {
        [TestMethod]
        public void ParseTest()
        {
            DefinationReader dr = new DefinationReader("Data\\test.def");
            dr.Read();
            StringBuilder sb = new StringBuilder();
            foreach (var section in dr.Sections)
            {
                Console.WriteLine("-----------------Sections--------------------");
                sb.AppendLine("-----------------Sections--------------------");
                foreach (var pair in section)
                {
                    Console.WriteLine("({0})=({1})", pair.Key, pair.Value);
                    sb.AppendLine(string.Format("({0})=({1})", pair.Key, pair.Value));
                }
            }

            string result = sb.ToString();
            Assert.IsTrue(result.Contains(@"(Name)=(abs)"));
            Assert.IsTrue(result.Contains(@"(Synopsis)="));
            Assert.IsTrue(result.Contains(@"(returns absolute value of scalars and vectors.)"));
            Assert.IsTrue(result.Contains(@"(Escape#Chars)=(\#=>#, \$=>$, \{=>{, \}=}, \==>=, \%=>%)"));
        }
    }
}
