using System.Collections.Generic;
using System.Xml;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;

[DataCollectorFriendlyName("CsDataCollector")]
[DataCollectorTypeUri("datacollector://Cryptosense/CsDataCollector")]
public class CsDataCollector : DataCollector, ITestExecutionEnvironmentSpecifier
{
    private string tracerPath32;
    private string tracerPath64;
    private string outputDir;

    public override void Initialize(
        XmlElement configurationElement,
        DataCollectionEvents events,
        DataCollectionSink dataSink,
        DataCollectionLogger logger,
        DataCollectionEnvironmentContext environmentContext)
    {
        tracerPath32 = configurationElement["TracerPath32"].InnerText;
        tracerPath64 = configurationElement["TracerPath64"].InnerText;
        outputDir = configurationElement["OutputDir"].InnerText;
    }

    public IEnumerable<KeyValuePair<string, string>> GetTestExecutionEnvironmentVariables()
    {
        return new List<KeyValuePair<string, string>> {
            new KeyValuePair<string, string>("COR_ENABLE_PROFILING", "1"),
            new KeyValuePair<string, string>("COR_PROFILER", "{cf0d821e-299b-5307-a3d8-9ccb4916d2e5}"),
            new KeyValuePair<string, string>("COR_PROFILER_PATH_32", tracerPath32),
            new KeyValuePair<string, string>("COR_PROFILER_PATH_64", tracerPath64),
            new KeyValuePair<string, string>("CS_OUTPUT_DIR", outputDir),
        };
    }
}
