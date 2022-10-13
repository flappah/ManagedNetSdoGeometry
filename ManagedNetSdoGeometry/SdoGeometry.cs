using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Globalization;
using System.Text;

namespace ManagedNetSdoGeometry
{
    [OracleCustomTypeMappingAttribute("MDSYS.SDO_GEOMETRY")]
    public class SdoGeometryFactory : IOracleCustomTypeFactory
    {
        public IOracleCustomType CreateObject()
        {
            return new SdoGeometry();
        }
    }
    public class SdoGeometry : INullable, IOracleCustomType
    {
        private enum OracleObjectColumns { SDO_GTYPE, SDO_SRID, SDO_POINT, SDO_ELEM_INFO, SDO_ORDINATES }

        private decimal? sdo_Gtype;

        [OracleObjectMappingAttribute(0)]
        public decimal? Sdo_Gtype
        {
            get { return sdo_Gtype; }
            set { sdo_Gtype = value; }
        }

        private decimal? sdo_Srid;

        [OracleObjectMappingAttribute(1)]
        public decimal? Sdo_Srid
        {
            get { return sdo_Srid; }
            set { sdo_Srid = value; }
        }

        private SdoPoint point;

        [OracleObjectMappingAttribute(2)]
        public SdoPoint Point
        {
            get { return point; }
            set { point = value; }
        }

        private decimal[] elemArray;

        [OracleObjectMappingAttribute(3)]
        public decimal[] ElemArray
        {
            get { return elemArray; }
            set { elemArray = value; }
        }

        private decimal[] ordinatesArray;

        [OracleObjectMappingAttribute(4)]
        public decimal[] OrdinatesArray
        {
            get { return ordinatesArray; }
            set { ordinatesArray = value; }
        }

        private bool _isNull;
        public virtual bool IsNull
        {
            get
            {
                return _isNull;
            }
        }

        public static SdoGeometry Null
        {
            get
            {
                SdoGeometry g = new SdoGeometry();
                g._isNull = true;
                return g;
            }
        }

        public void FromCustomObject(OracleConnection con, object udt)
        {
            OracleUdt.SetValue(con, udt, (int)OracleObjectColumns.SDO_GTYPE, Sdo_Gtype);
            OracleUdt.SetValue(con, udt, (int)OracleObjectColumns.SDO_SRID, Sdo_Srid);
            OracleUdt.SetValue(con, udt, (int)OracleObjectColumns.SDO_POINT, Point);
            OracleUdt.SetValue(con, udt, (int)OracleObjectColumns.SDO_ELEM_INFO, ElemArray);
            OracleUdt.SetValue(con, udt, (int)OracleObjectColumns.SDO_ORDINATES, OrdinatesArray);
        }

        public void ToCustomObject(OracleConnection con, object udt)
        {
            Sdo_Gtype = (decimal?)OracleUdt.GetValue(con, udt, (int)OracleObjectColumns.SDO_GTYPE);
            Sdo_Srid = (decimal?)OracleUdt.GetValue(con, udt, (int)OracleObjectColumns.SDO_SRID);
            Point = (SdoPoint)OracleUdt.GetValue(con, udt, (int)OracleObjectColumns.SDO_POINT);
            ElemArray = (decimal[])OracleUdt.GetValue(con, udt, (int)OracleObjectColumns.SDO_ELEM_INFO);
            OrdinatesArray = (decimal[])OracleUdt.GetValue(con, udt, (int)OracleObjectColumns.SDO_ORDINATES);
        }

        #region WKT enhancements

        /// <summary>
        ///     Returns the WKT of the stored SDO_GEOMETRY
        /// </summary>
        /// <returns>string</returns>
        public string Get_WKT()
        {
            switch (Sdo_Gtype)
            {
                case 2001:
                    return Get_PointWKT();

                case 2002:
                    return Get_LineStringWKT();

                case 2003:
                    return Get_PolygonWKT();

                case 3001:
                    return Get_3DPointWKT();

                case 3005:
                    return Get_3DPointSetWKT();

                default:
                    return String.Empty;
            }
        }

        /// <summary>
        /// Returns the POINT WKT
        /// </summary>
        /// <returns>string</returns>
        private string Get_PointWKT()
        {
            if (OrdinatesArray == null || OrdinatesArray.Length != 2)
            {
                if (Point.IsNull)
                {
                    return String.Empty; // only two coordinates allowed
                }

                OrdinatesArray = new decimal[] { (decimal)Point.X, (decimal)Point.Y };
            }

            return $"POINT ({OrdinatesArray[0].ToString(new CultureInfo("en-US"))} {OrdinatesArray[1].ToString(new CultureInfo("en-US"))})";
        }

        /// <summary>
        /// Returns the 3D representation of a POINT WKT
        /// </summary>
        /// <returns>string</returns>
        private string Get_3DPointWKT()
        {
            if (OrdinatesArray == null || OrdinatesArray.Length != 3)
            {
                if (Point.IsNull)
                {
                    return String.Empty;
                }

                OrdinatesArray = new decimal[] { (decimal)Point.X, (decimal)Point.Y, (decimal)Point.Z };
            }

            return $"POINT M ({OrdinatesArray[0].ToString(new CultureInfo("en-US"))} {OrdinatesArray[1].ToString(new CultureInfo("en-US"))} {OrdinatesArray[2].ToString().Replace(",", ".")})";
        }

        /// <summary>
        /// Returns a POINT M GEOMETRYCOLLECTION WKT
        /// </summary>
        /// <returns>string</returns>
        private string Get_3DPointSetWKT()
        {
            if (OrdinatesArray == null || OrdinatesArray.Length < 6)
            {
                return String.Empty;
            }

            var sb = new StringBuilder();

            sb.Append("GEOMETRYCOLLECTION (");
            for (int i = 0; i < OrdinatesArray.Length; i += 3)
            {
                if (i != 0)
                {
                    sb.Append(",");
                }

                sb.Append($"POINT M ({OrdinatesArray[i].ToString(new CultureInfo("en-US"))} {OrdinatesArray[i + 1].ToString(new CultureInfo("en-US"))} {OrdinatesArray[i + 2].ToString(new CultureInfo("en-US"))})");
            }
            sb.Append(")");

            return sb.ToString();
        }

        /// <summary>
        /// Returns the LINESTRING WKT
        /// </summary>
        /// <returns>string</returns>
        private string Get_LineStringWKT()
        {
            if (OrdinatesArray == null || OrdinatesArray.Length < 4 || OrdinatesArray.Length % 2 != 0)
            {
                return String.Empty; // use only non empty valid coordinate pairs that have at least two pairs
            }

            var sb = new StringBuilder();

            sb.Append("LINESTRING (");
            for (int i = 0; i < OrdinatesArray.Length; i += 2)
            {
                if (i != 0)
                {
                    sb.Append(",");
                }

                sb.Append($"{OrdinatesArray[i].ToString(new CultureInfo("en-US"))} {OrdinatesArray[i + 1].ToString(new CultureInfo("en-US"))}");
            }
            sb.Append(")");

            return sb.ToString();
        }

        /// <summary>
        /// Returns the POLYGON WKT
        /// </summary>
        /// <returns>string</returns>
        private string Get_PolygonWKT()
        {
            if (OrdinatesArray == null || OrdinatesArray.Length == 0 || ElemArray.Length == 0)
            {
                return String.Empty;
            }

            if (ElemArray.Length == 3) // no inner rings
            {
                if (OrdinatesArray[0] == OrdinatesArray[OrdinatesArray.Length - 2] &&
                    OrdinatesArray[1] == OrdinatesArray[OrdinatesArray.Length - 1]) // area has to be closed
                {
                    return Get_PolygonNoInnerRingsWKT();
                }

                // if not, a linestring is returned
                return Get_LineStringWKT();
            }

            return Get_PolygonWithInnerRingsWKT(); // polygon has inner rings
        }

        /// <summary>
        /// POLYGON WKT having no inner rings. 
        /// </summary>
        /// <returns>string</returns>
        private string Get_PolygonNoInnerRingsWKT()
        {
            if (OrdinatesArray == null || OrdinatesArray.Length < 4 || OrdinatesArray.Length % 2 != 0)
            {
                return String.Empty;
            }

            var sb = new StringBuilder();

            sb.Append("POLYGON ((");

            for (int i = 0; i < OrdinatesArray.Length; i += 2)
            {
                if (i != 0)
                {
                    sb.Append(",");
                }

                sb.Append($"{OrdinatesArray[i].ToString(new CultureInfo("en-US"))} {OrdinatesArray[i + 1].ToString(new CultureInfo("en-US"))}");
            }
            sb.Append("))");

            return sb.ToString();
        }

        /// <summary>
        /// POLYGON WKT having one or multiple inner rings
        /// </summary>
        /// <returns>string</returns>
        private string Get_PolygonWithInnerRingsWKT()
        {
            if (OrdinatesArray == null || OrdinatesArray.Length < 4 || OrdinatesArray.Length % 2 != 0)
            {
                return String.Empty;
            }

            var sb = new StringBuilder();

            sb.Append("POLYGON (");

            decimal firstX = 0;
            decimal firstY = 0;
            bool addSeparator = false;
            for (int i = 0; i < OrdinatesArray.Length; i += 2)
            {
                if (firstX == OrdinatesArray[i] && firstY == OrdinatesArray[i + 1])
                {
                    firstX = 0;
                    firstY = 0;
                    sb.Append(",");
                    sb.Append($"{OrdinatesArray[i].ToString(new CultureInfo("en-US"))} {OrdinatesArray[i + 1].ToString(new CultureInfo("en-US"))}");
                    sb.Append(")");
                }
                else
                {
                    if (firstX == 0 && firstY == 0)
                    {
                        if (i != 0)
                        {
                            sb.Append(",");
                        }

                        addSeparator = false;
                        firstX = OrdinatesArray[i];
                        firstY = OrdinatesArray[i + 1];
                        sb.Append("(");
                    }

                    if (addSeparator)
                    {
                        sb.Append(",");
                    }

                    sb.Append($"{OrdinatesArray[i].ToString(new CultureInfo("en-US"))} {OrdinatesArray[i + 1].ToString(new CultureInfo("en-US"))}");
                    addSeparator = true;
                }
            }

            sb.Append(")");

            return sb.ToString();
        }

        #endregion
    }
}
