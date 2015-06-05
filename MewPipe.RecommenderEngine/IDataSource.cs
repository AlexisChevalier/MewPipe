using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MewPipe.RecommenderEngine
{
    public interface IDataSource
    {
        DataSourceResult GetData();
        void SaveData(string videoId, KeyValuePair<string, double>[] results);
    }
}
