using Oracle.ManagedDataAccess.Types;
using System;

namespace ManagedNetSdoGeometry
{
    [OracleCustomTypeMappingAttribute("MDSYS.SDO_ELEM_INFO_ARRAY")]
    public class ElemArrayFactory : IOracleArrayTypeFactory
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
