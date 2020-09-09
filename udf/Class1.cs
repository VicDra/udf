using Microsoft.SqlServer.Server;
using System.Text.RegularExpressions;
using System.Data.SqlTypes;
using System.Collections;
using System.Collections.Generic;

namespace udf
{

    public partial class StoredFunctions
    {
        [SqlFunction(IsDeterministic = true, IsPrecise = true)]
        public static SqlBoolean RegExMatch(SqlString input, SqlString pattern)
        {
            if (input.IsNull || pattern.IsNull) 
                return SqlBoolean.False;

            return Regex.IsMatch(input.Value, pattern.Value, RegexOptions.IgnoreCase);
        }

        [SqlFunction(IsDeterministic = true, IsPrecise = true)]
        public static SqlString RegExReplace(SqlString input, SqlString pattern, SqlString replacement)
        {
            if (input.IsNull || pattern.IsNull || replacement.IsNull)
                return SqlString.Null;

            return new SqlString(Regex.Replace(input.Value, pattern.Value, replacement.Value, RegexOptions.IgnoreCase));
        }
        
        [SqlFunction(DataAccess = DataAccessKind.Read, FillRowMethodName = "FillMatches", TableDefinition = "GroupNumber int, MatchText nvarchar(4000)")]
        public static IEnumerable RegexGroupValues(string Input, string Pattern)
        {
            List<RegexMatch> GroupCollection = new List<RegexMatch>();

            Match m = Regex.Match(Input, Pattern);
            if (m.Success)
            {
                for (int i = 0; i < m.Groups.Count; i++)
                {
                    GroupCollection.Add(new RegexMatch(i, m.Groups[i].Value));
                }
            }

            return GroupCollection;
        }

        public static void FillMatches(object Group, out SqlInt32 GroupNumber, out SqlString MatchText)
        {
            RegexMatch rm = (RegexMatch)Group;
            GroupNumber = rm.GroupNumber;
            MatchText = rm.MatchText;
        }

        private class RegexMatch
        {
            public SqlInt32 GroupNumber { get; set; }
            public SqlString MatchText { get; set; }

            public RegexMatch(SqlInt32 group, SqlString match)
            {
                this.GroupNumber = group;
                this.MatchText = match;
            }
        }
    }
}
