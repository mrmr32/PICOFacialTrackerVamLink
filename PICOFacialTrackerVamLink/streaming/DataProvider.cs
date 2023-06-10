using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PICOFacialTrackerVamLink;

public interface DataProvider<T>
{
    bool IsNewDataAvailable();
    IDictionary<T, float> GetData();
}