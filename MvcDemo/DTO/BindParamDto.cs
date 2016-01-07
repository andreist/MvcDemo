using System;

namespace MvcDemo
{
    [Serializable]
    public class BindDataParamDto
    {
        public int Page { get; set; }
        public int Rows { get; set; }
        public string Sidx { get; set; }
        public string Sord { get; set; }

        private string _filters;
        public string Filters
        {
            get { return _filters; }
            set { _filters = value; }
        }

        public string Search
        {
            get { return (!string.IsNullOrEmpty(_filters)).ToString().ToLower(); }
        }
    }
}
