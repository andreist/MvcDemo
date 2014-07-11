using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MvcDemo.Common
{
    public class CustomDataSource<T> where T : class 
    {
        public CustomDataSource(List<T> recordList, int totalRecords)
        {
            TotalRecords = totalRecords;
            RecordList = recordList;
        }

        public int TotalRecords { get; set; }
        public List<T> RecordList { get; set; }
      
    }
}
