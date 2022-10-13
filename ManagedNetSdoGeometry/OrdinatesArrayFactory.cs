using Oracle.ManagedDataAccess.Types;
using System;

namespace ManagedNetSdoGeometry
{
    [OracleCustomTypeMappingAttribute("MDSYS.SDO_ORDINATE_ARRAY")]
    public class OrdinatesArrayFactory : IOracleArrayTypeFactory
    {
        public Array CreateArray(int numElems)
        {
            return new decimal[numElems];
        }

        public Array CreateStatusArray(int numElems)
        {
            return new decimal[numElems];
        }
    }
}
