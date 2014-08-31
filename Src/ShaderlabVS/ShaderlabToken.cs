using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaderlabVS
{
    public enum ShaderlabToken
    {
        TEXT,
        COMMENT,
        STRING_LITERAL,
        HLSLCGKEYWORD,
        HLSLCGKEYWORDSPECIAL,
        HLSLCGDATATYPE,
        HLSLCGFUNCTION,
        UNITYKEYWORD,
        UNITYKEYWORD_PARA,
        UNITYVALUES,
        UNITYDATATYPE,
        UNITYFUNCTION,
        UNITYMACROS,
        UNDEFINED,
    }
}
