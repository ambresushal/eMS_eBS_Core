using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels
{
    public class GridPagingRequest
    {
        public string sidx { get; set; }
        public string sord { get; set; }
        public int page { get; set; }
        public int rows { get; set; }
        public bool _search { get; set; }
        public string filters { get; set; }
        public List<GridPagingRequest> ruls{get ;set;}
    }

   

    public class GridPagingResponse<T>
    {
        private int _total;
        private int _page;
        private int _records;
        private T[] _rows;

        public GridPagingResponse(int page, int totalCount, int rowCount, IList<T> rows)
        {
            this._page = page;
            this._records = totalCount;
            this._total = (int)Math.Ceiling((float)totalCount / (float)rowCount);
            this._rows = (rows != null) ? rows.ToArray<T>() : null; 
        }

        public int total { get { return this._total; } }
        public int page { get { return this._page; } }
        public int records { get { return this._records; } }
        public T[] rows { get { return this._rows; } }

    }

}
