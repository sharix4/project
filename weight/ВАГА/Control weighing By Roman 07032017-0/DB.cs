using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Control_weighing_By_Roman_07032017_0
{
    class DB
    {
   
        #region SQL queries
        static Dictionary<string, string> sql_query_dict = new Dictionary<string, string>
    { {"Перевірити зєднання",@"
/****** Script for SelectTopNRows command from SSMS  ******/
SELECT TOP 1 [ID_calc]
      ,[date_work]
      ,[name_rec]
      ,[ID_rec]
      ,[part_name]
      ,[ID_part]
      ,[amount]
      ,[note]
      ,[time_work]
  FROM [weight].[dbo].[how_much]"
},{"Записати дані в базу",@"
INSERT INTO [dbo].[how_much]
           ([date_work]
           ,[name_rec]
           ,[ID_rec]
           ,[part_name]
           ,[ID_part]
           ,[amount]
           ,[note]
           ,[time_work])
     VALUES
           (@@date_work
           ,@@name_rec
           ,@@ID_rec
           ,@@part_name
           ,@@ID_part
           ,@@amount
           ,@@note
           ,@@time_work)

		select(SCOPE_IDENTITY()) as ID"
  },{"рецепт в базу",
@"
INSERT INTO [dbo].[weighting]
           ([ID_calc]
           ,[date_work]
           ,[name]
           ,[ID_name]
           ,[amount]
           ,[amount_real]
           ,[time_weighting])
     VALUES
           (@@ID_calc
           ,@@date_work
           ,@@name
           ,@@ID_name
           ,@@amount
           ,@@amount_real
           ,@@time_weighting
		   )
select('good')as suses;"},
                        {"get_how_much",@"

DECLARE @ID int

set @ID=@@ID_calc
 
 SELECT [ID_calc]
      ,[date_work]
      ,[name_rec]
      ,[ID_rec]
      ,[part_name]
      ,[ID_part]
      ,[amount]
      ,[note]
      ,[time_work]
  FROM [weight].[dbo].[how_much]
  where  
  CASE 
		 WHEN @ID IS NULL THEN ' ' ELSE [dbo].[how_much].[ID_calc] END  >
  CASE 
         WHEN @ID IS NULL THEN ' ' ELSE @ID END "},
                                 {"get_weighting",

@"

DECLARE @ID int

set @ID    =@@ID_weighting

SELECT [ID_weighting]
      ,[ID_calc]
      ,[date_work]
      ,[name]
      ,[ID_name]
      ,[amount]
      ,[amount_real]
      ,[time_weighting]
  FROM [weight].[dbo].[weighting]
 where  
  CASE 
		 WHEN @ID IS NULL THEN ' ' ELSE [weight].[dbo].[weighting].[ID_weighting] END  >
  CASE 
         WHEN @ID IS NULL THEN ' ' ELSE @ID END "}};
        #endregion




        private static string default_connection = "Data Source=.\\SQLEXPRESS;Initial Catalog= weight;Integrated Security=True";
 
        private string used_connection = null;

        public DB()
        {
            option set_data = new option();
            used_connection = default_connection; //set_data.load_config_seting("bd_string");
        }

        public bool Updeit(string command, DataTable parameters)
        {
            using (SqlConnection conection = new SqlConnection(used_connection))
            {
                conection.Open();
                using (SqlCommand Insert = conection.CreateCommand())
                {
                    Insert.CommandType = CommandType.Text;
                    string text = sql_query_dict[command];
                    Insert.CommandText = text;
                    if (parameters != null)
                    {
                        string pat = @"\@(\w+)";

                        // Instantiate the regular expression object.
                        Regex r = new Regex(pat, RegexOptions.IgnoreCase);

                        // Match the regular expression pattern against a text string.
                        Match m = r.Match(text);
                        while (m.Success)
                        {
                            foreach (DataRow row in parameters.Rows)
                            {
                                if (m.Groups[1].Value == "")
                                { break; }
                                var item = row[m.Groups[1].Value];

                                if (item is int)
                                {
                                    Insert.Parameters.Add("@" + m.Groups[1].Value, SqlDbType.Int)
                                     .Value = item;
                                }
                                else if (item is String)
                                {
                                    Insert.Parameters.Add("@" + m.Groups[1].Value, SqlDbType.VarChar)
                                     .Value = item;
                                }
                                else if (item is Decimal)
                                {
                                    Insert.Parameters.Add("@" + m.Groups[1].Value, SqlDbType.Money)
                                         .Value = item;
                                }
                                else if (item is DateTime)
                                {
                                        Insert.Parameters.Add("@" + m.Groups[1].Value, SqlDbType.DateTime2)
                                           .Value = item;

                                }
                                else if (item is float)
                                {
                                    Insert.Parameters.Add("@" + m.Groups[1].Value, SqlDbType.Float)
                                                .Value = item;

                                }
                                else if (item is double)
                                {
                                    Insert.Parameters.Add("@" + m.Groups[1].Value, SqlDbType.Float)
                                                    .Value = item;
                                }
                                if (item is TimeSpan)
                                {
                                    Insert.Parameters.Add("@" + m.Groups[1].Value, SqlDbType.Time)
                                                .Value = item;

                                }
                                m = m.NextMatch();
                            }

                        }
                        Insert.ExecuteNonQuery();
                    }
                }
            }
            return true;
        }

        public bool Delete(string command, DataTable parameters)
        {
            using (SqlConnection conection = new SqlConnection(used_connection))
            {
                conection.Open();
                using (SqlCommand Insert = conection.CreateCommand())
                {
                    Insert.CommandType = CommandType.Text;
                    string text = sql_query_dict[command];
                    Insert.CommandText = text;
                    if (parameters != null)
                    {
                        string pat = @"\@(\w+)";
                        Regex r = new Regex(pat, RegexOptions.IgnoreCase);
                        Match m = r.Match(text);
                        while (m.Success)
                        {
                            foreach (DataRow row in parameters.Rows)
                            {
                                if (m.Groups[1].Value == "")
                                { break; }
                                var item = row[m.Groups[1].Value];

                                if (item is int)
                                {
                                    Insert.Parameters.Add("@" + m.Groups[1].Value, SqlDbType.Int)
                                     .Value = item;
                                }
                                else if (item is String)
                                {
                                    Insert.Parameters.Add("@" + m.Groups[1].Value, SqlDbType.VarChar)
                                     .Value = item;
                                }
                                else if (item is Decimal)
                                {
                                    Insert.Parameters.Add("@" + m.Groups[1].Value, SqlDbType.Money)
                                         .Value = item;
                                }
                                else if (item is DateTime)
                                {
                                    if (m.Groups[1].Value == "date")
                                    {
                                        Insert.Parameters.Add("@" + m.Groups[1].Value, SqlDbType.DateTime2)
                                           .Value = item;
                                    }
                                    else
                                    {
                                        Insert.Parameters.Add("@" + m.Groups[1].Value, SqlDbType.Date)
                                              .Value = item;
                                    }
                                }
                                else if (item is float)
                                {
                                    Insert.Parameters.Add("@" + m.Groups[1].Value, SqlDbType.Float)
                                                .Value = item;

                                }
                                m = m.NextMatch();
                            }

                        }
                        Insert.ExecuteNonQuery();
                    }
                }
            }
            return true;
        }

        public bool insert_new(string command, DataTable parameters)
        {

            using (SqlConnection conection = new SqlConnection(used_connection))
            {
                conection.Open();
                using (SqlCommand Insert = conection.CreateCommand())
                {
                    Insert.CommandType = CommandType.Text;

                    string text = sql_query_dict[command];
                    Insert.CommandText = text;
                    if (parameters != null)
                    {
                        string pat = @"\@@(\w+)";

                        // Instantiate the regular expression object.
                        Regex r = new Regex(pat, RegexOptions.IgnoreCase);

                        // Match the regular expression pattern against a text string.
                        Match m = r.Match(text);
                        while (m.Success)
                        {
                            foreach (DataRow row in parameters.Rows)
                            {
                                if (m.Groups[1].Value == "")
                                { break; }
                                var item = row[m.Groups[1].Value];

                                if (item is int)
                                {
                                    Insert.Parameters.Add("@@" + m.Groups[1].Value, SqlDbType.Int)
                                     .Value = item;
                                }
                                if (item is String)
                                {
                                    Insert.Parameters.Add("@@" + m.Groups[1].Value, SqlDbType.NVarChar)
                                     .Value = item;
                                }
                                if (item is Decimal)
                                {
                                    Insert.Parameters.Add("@@" + m.Groups[1].Value, SqlDbType.Money)
                                         .Value = item;
                                }
                                if (item is DateTime)
                                {
                                    if (m.Groups[1].Value == "date")
                                    {
                                        Insert.Parameters.Add("@@" + m.Groups[1].Value, SqlDbType.DateTime2)
                                           .Value = item;
                                    }
                                    else
                                    {
                                        Insert.Parameters.Add("@@" + m.Groups[1].Value, SqlDbType.Date)
                                              .Value = item;
                                    }
                                }
                                if (item is float)
                                {
                                    Insert.Parameters.Add("@@" + m.Groups[1].Value, SqlDbType.Float)
                                                .Value = item;

                                }
                                if (item is double)
                                {
                                    Insert.Parameters.Add("@@" + m.Groups[1].Value, SqlDbType.Float)
                                                .Value = item;

                                }
                                if (item is TimeSpan)
                                {
                                    Insert.Parameters.Add("@@" + m.Groups[1].Value, SqlDbType.Time)
                                                .Value = item;

                                }
                                if (item is DBNull)
                                {
                                    Insert.Parameters.Add("@@" + m.Groups[1].Value, SqlDbType.NVarChar)
                                                 .Value = DBNull.Value;
                                }
                                m = m.NextMatch();
                            }

                        }
                        Insert.ExecuteNonQuery();
                    }
                }
            }

            return true;
        }

        public DataTable select_new(string command, DataTable parameters)
        {
            DataTable table = new DataTable();

            using (SqlConnection conection = new SqlConnection(used_connection))
            {
                conection.Open();
                using (SqlCommand newCmd = conection.CreateCommand())
                {
                    newCmd.CommandType = CommandType.Text;
                    string text = sql_query_dict[command];
                    newCmd.CommandText = text;
                    if (parameters != null)
                    {
                        string pat = @"\@@(\w+)";

                        // Instantiate the regular expression object.
                        Regex r = new Regex(pat, RegexOptions.IgnoreCase);

                        // Match the regular expression pattern against a text string.
                        Match m = r.Match(text);
                        //int matchCount = 0;
                        while (m.Success)
                        {
                            foreach (DataRow row in parameters.Rows)
                            {
                                if (m.Groups[1].Value == "")
                                { break; }
                                var item = row[m.Groups[1].Value];

                                if (item is Int32)
                                {
                                    newCmd.Parameters.Add("@@" + m.Groups[1].Value, SqlDbType.Int)
                                     .Value = item;
                                }
                                if (item is Decimal)
                                {
                                    newCmd.Parameters.Add("@@" + m.Groups[1].Value, SqlDbType.Money)
                                        .Value = item;
                                }
                                if (item is String)
                                {
                                    newCmd.Parameters.Add("@@" + m.Groups[1].Value, SqlDbType.VarChar)
                                     .Value = item;
                                }
                                if (item is float)
                                {
                                    newCmd.Parameters.Add("@@" + m.Groups[1].Value, SqlDbType.Float)
                                        .Value = item;
                                }
                                if (item is DateTime)
                                {
                                    newCmd.Parameters.Add("@@" + m.Groups[1].Value, SqlDbType.DateTime2)
                                     .Value = item;
                                }
                                if (item is double)
                                {
                                    newCmd.Parameters.Add("@@" + m.Groups[1].Value, SqlDbType.Float)
                                                 .Value = item;
                                }
                                if (item is TimeSpan)
                                {
                                    newCmd.Parameters.Add("@@" + m.Groups[1].Value, SqlDbType.Time)
                                                .Value = item;

                                }
                                if (item is DBNull)
                                {
                                    newCmd.Parameters.Add("@@" + m.Groups[1].Value, SqlDbType.NVarChar)
                                                 .Value = DBNull.Value;
                                }
                                m = m.NextMatch();
                            }
                        }
                    }
                    using (SqlDataReader rdr = newCmd.ExecuteReader())
                    {
                        table.Load(rdr);
                    }
                }
            }
            return table;
        }
    }
}
