using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace ManagedNetSdoGeometry
{
    [OracleCustomTypeMappingAttribute("MDSYS.SDO_POINT_TYPE")]
    public class SdoPointFactory : IOracleCustomTypeFactory
    {
        public IOracleCustomType CreateObject()
        {
            return new SdoPoint();
        }
    }
    public class SdoPoint : INullable, IOracleCustomType
    {
        private decimal? x;

        [OracleObjectMappingAttribute("X")]
        public decimal? X
        {
            get { return x; }
            set { x = value; }
        }

        private decimal? y;

        [OracleObjectMappingAttribute("Y")]
        public decimal? Y
        {
            get { return y; }
            set { y = value; }
        }

        private decimal? z;

        [OracleObjectMappingAttribute("Z")]
        public decimal? Z
        {
            get { return z; }
            set { z = value; }
        }

        private bool _isNull;

        public virtual bool IsNull
        {
            get
            {
                return _isNull;
            }
        }

        public static SdoPoint Null
        {
            get
            {
                SdoPoint p = new SdoPoint();
                p._isNull = true;
                return p;
            }
        }

        public void FromCustomObject(OracleConnection con, object udt)
        {
            OracleUdt.SetValue(con, udt, "X", x);
            OracleUdt.SetValue(con, udt, "Y", y);
            OracleUdt.SetValue(con, udt, "Z", z);
        }

        public void ToCustomObject(OracleConnection con, object udt)
        {
            X = (decimal?)OracleUdt.GetValue(con, udt, "X");
            Y = (decimal?)OracleUdt.GetValue(con, udt, "Y");
            Z = (decimal?)OracleUdt.GetValue(con, udt, "Z");
        }
    }
}
