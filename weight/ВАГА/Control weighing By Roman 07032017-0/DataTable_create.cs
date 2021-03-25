using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control_weighing_By_Roman_07032017_0
{
    class DataTable_create
    {

        public DataTable how_much()
        {
            DataTable data_search = new DataTable();
            data_search.TableName = "how_much";
            data_search.Columns.Add("date_work", typeof(DateTime));
            data_search.Columns.Add("name_rec", typeof(string));
            data_search.Columns.Add("ID_rec", typeof(int));
            data_search.Columns.Add("part_name", typeof(string));
            data_search.Columns.Add("ID_part", typeof(int));
            data_search.Columns.Add("amount", typeof(decimal));
            data_search.Columns.Add("note", typeof(string));
            data_search.Columns.Add("time_work", typeof(TimeSpan));
            return data_search;
        }

        public DataTable weighting()
        {
            DataTable data_search = new DataTable();
            data_search.TableName = "weighting";
            data_search.Columns.Add("ID_calc", typeof(int));
            data_search.Columns.Add("date_work", typeof(DateTime));
            data_search.Columns.Add("name", typeof(string));
            data_search.Columns.Add("ID_name", typeof(int));
            data_search.Columns.Add("amount", typeof(decimal));
            data_search.Columns.Add("amount_real", typeof(decimal));
            data_search.Columns.Add("time_weighting", typeof(TimeSpan));
            return data_search;
        }



        public DataTable number()
        {
            DataTable data_search = new DataTable();
            data_search.TableName = "number";
            data_search.Columns.Add("start", typeof(string));
            data_search.Columns.Add("do_zamis", typeof(string));
            data_search.Columns.Add("remainder", typeof(string));
            data_search.Columns.Add("part_name", typeof(string));
            return data_search;
        }
    }
}
