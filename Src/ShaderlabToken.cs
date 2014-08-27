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
        COMMENT_LINE,
        COMMENT,
        NUMBER,
        STRING_LITERAL,
        FLOAT,
        KEYWORD,
        KEYWORDSPECIAL,
        UNITYBLOCKKEYWORD,
        UNITYNONBLOCKKEYWORD,
        FUNCTION,
        UNITYFUNCTION,
        DATATYPE,
        SYMBOL,
        UNDEFINED,
    }
}
