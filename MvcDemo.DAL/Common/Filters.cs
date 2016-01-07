using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Data.Entity.Core.Objects;

namespace MvcDemo.DAL
{
    public class Filters
    {
        #region Enums

        public enum GroupOp
        {
            AND,
            OR
        }

        public enum Operations
        {
            eq, // "equal"
            ne, // "not equal"
            lt, // "less"
            le, // "less or equal"
            gt, // "greater"
            ge, // "greater or equal"
            bw, // "begins with"
            bn, // "does not begin with"
            //in, // "in"
            //ni, // "not in"
            ew, // "ends with"
            en, // "does not end with"
            cn, // "contains"
            nc  // "does not contain"
        }

        #endregion


        #region Fileds

        public GroupOp groupOp { get; set; }

        public List<Rule> rules { get; set; }

        // ReSharper restore InconsistentNaming
        private static readonly string[] FormatMapping = {
            "(it.{0} = @p{1})",                 // "eq" - equal
            "(it.{0} <> @p{1})",                // "ne" - not equal
            "(it.{0} < @p{1})",                 // "lt" - less than
            "(it.{0} <= @p{1})",                // "le" - less than or equal to
            "(it.{0} > @p{1})",                 // "gt" - greater than
            "(it.{0} >= @p{1})",                // "ge" - greater than or equal to
            "(it.{0} LIKE (@p{1}+'%'))",        // "bw" - begins with
            "(it.{0} NOT LIKE (@p{1}+'%'))",    // "bn" - does not begin with
            "(it.{0} LIKE ('%'+@p{1}))",        // "ew" - ends with
            "(it.{0} NOT LIKE ('%'+@p{1}))",    // "en" - does not end with
            "(it.{0} LIKE ('%'+@p{1}+'%'))",    // "cn" - contains
            "(it.{0} NOT LIKE ('%'+@p{1}+'%'))" //" nc" - does not contain
        };

        #endregion


        #region Nested Classes

        public class Rule
        {
            public string field { get; set; }
            public Operations op { get; set; }
            public string data { get; set; }
        }

        #endregion


        #region Methods

        public ObjectQuery<T> FilterObjectSet<T>(ObjectQuery<T> inputQuery) where T : class
        {
            if (rules.Count <= 0)
                return inputQuery;

            var sb = new StringBuilder();
            var objParams = new List<ObjectParameter>(rules.Count);

            foreach (var rule in rules)
            {
                var propertyInfo = GetPropertyInfo(rule.field, typeof (T));
                if (propertyInfo == null)
                    continue; // skip, wrong entries

                var iParam = objParams.Count;
                bool validParam = true;
                ObjectParameter param = null;

                switch (propertyInfo.PropertyType.FullName)
                {
                    case "System.Int32": // int
                        Int32 iOutInt32;
                        if (Int32.TryParse(rule.data, out iOutInt32))
                            param = new ObjectParameter("p" + iParam, iOutInt32);
                        else
                            validParam = false;
                        break;
                    case "System.Int64": // bigint
                        Int64 iOutInt64;
                        if (Int64.TryParse(rule.data, out iOutInt64))
                            param = new ObjectParameter("p" + iParam, iOutInt64);
                        else
                            validParam = false;
                        break;
                    case "System.Int16": // smallint
                        Int64 iOutInt16;
                        if (Int64.TryParse(rule.data, out iOutInt16))
                            param = new ObjectParameter("p" + iParam, iOutInt16);
                        else
                            validParam = false;
                        break;
                    case "System.SByte": // tinyint
                        SByte sbOut;
                        if (SByte.TryParse(rule.data, out sbOut))
                            param = new ObjectParameter("p" + iParam, sbOut);
                        else
                            validParam = false;
                        break;
                    case "System.Single": // Edm.Single, in SQL: float
                        Single sgOut;
                        if (Single.TryParse(rule.data, out sgOut))
                            param = new ObjectParameter("p" + iParam, sgOut);
                        else
                            validParam = false;
                        break;
                    case "System.Double": // float(53), double precision
                        Double dOut;
                        if (Double.TryParse(rule.data, out dOut))
                            param = new ObjectParameter("p" + iParam, dOut);
                        else
                            validParam = false;
                        break;
                    case "System.Boolean": // Edm.Boolean, in SQL: bit
                        param = new ObjectParameter("p" + iParam,
                                                    String.Compare(rule.data, "1", StringComparison.Ordinal) == 0 ||
                                                    String.Compare(rule.data, "yes", StringComparison.OrdinalIgnoreCase) ==
                                                    0 ||
                                                    String.Compare(rule.data, "true", StringComparison.OrdinalIgnoreCase) ==
                                                    0);
                        break;
                    case "System.DateTime":
                        DateTime dtOut;
                        if (DateTime.TryParse(rule.data, out dtOut))
                            param = new ObjectParameter("p" + iParam, dtOut);
                        else
                            validParam = false;
                        break;
                    default:
                        // TODO: Extend to other data types
                        // binary, date, datetimeoffset,
                        // decimal, numeric,
                        // money, smallmoney
                        // and so on

                        param = new ObjectParameter("p" + iParam, rule.data);
                        break;
                }

                if (validParam)
                {
                    if (sb.Length != 0)
                        sb.Append(groupOp);
                    sb.AppendFormat(FormatMapping[(int) rule.op], rule.field, iParam);

                    objParams.Add(param);
                }
            }

            if (sb.Length == 0)
                return inputQuery;

            var filteredQuery = inputQuery.Where(sb.ToString());
            foreach (var objParam in objParams)
                filteredQuery.Parameters.Add(objParam);

            return filteredQuery;
        }

        private PropertyInfo GetPropertyInfo(string propertyName, Type propertyType)
        {
            PropertyInfo propertyInfo = null;

            string[] propertyNameSlited = propertyName.Split('.');
            foreach (string pn in propertyNameSlited)
            {
                propertyInfo = propertyType.GetProperty(pn);
                if (propertyInfo == null)
                    return null;

                propertyType = propertyInfo.PropertyType;
            }
            return propertyInfo;
        }


        #endregion

    }

}