using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ShaderlabVS.Data
{
    public class DefinationDataProvider<T> where T : ModelBase
    {
        public static List<T> ProvideFromFile(string defFileName)
        {
            DefinationReader dr = new DefinationReader(defFileName);
            dr.Read();
            List<T> list = new List<T>();
            Type t = typeof(T);

            if (t == null)
            {
                throw new TypeLoadException(String.Format("Cannot find Type {0}", t.ToString()));
            }

            foreach (var section in dr.Sections)
            {
                T tInstance = t.Assembly.CreateInstance(t.ToString()) as T;
                if (tInstance == null)
                {
                    throw new TypeLoadException(String.Format("Create Type {0} failed", t.ToString()));
                }

                // Set the property value to the instance of T 
                //
                foreach (PropertyInfo property in t.GetProperties())
                {
                    // Get DefinationKey attribute
                    //
                    DefinationKeyAttribute dkattr = property.GetCustomAttributes(typeof(DefinationKeyAttribute)).FirstOrDefault() as DefinationKeyAttribute;
                    if (dkattr != null)
                    {
                        // get value in 
                        //
                        if (section.ContainsKey(dkattr.Name))
                        {
                            string value = section[dkattr.Name];
                            property.SetValue(tInstance, value);
                        }
                    }
                }

                ModelBase mb = tInstance as ModelBase;
                mb.PrepareProperties();

                list.Add(tInstance);
            }

            return list;
        }
    }
}
