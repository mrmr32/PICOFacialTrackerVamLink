using System.Collections.Generic;
using System.Linq;

namespace PICOFacialTrackerVamLink;
public abstract class VAMConverter<T> : DataProvider<string>
{
    protected DataProvider<T> provider;

    public VAMConverter(DataProvider<T> provider)
    {
        this.provider = provider;
    }

    public abstract IDictionary<string, float> GetData();

    public string GetJSONData()
    {
        return "{" + string.Join(",", this.GetData().Select(x => "\"" + x.Key + "\":" + x.Value.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)).ToArray()) + "}";
    }

    public bool IsNewDataAvailable()
    {
        return this.provider.IsNewDataAvailable();
    }
}