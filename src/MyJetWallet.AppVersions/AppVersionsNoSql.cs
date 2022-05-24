using Microsoft.AspNetCore.WebUtilities;
using MyNoSqlServer.Abstractions;

namespace MyJetWallet.AppVersions;

public class AppVersionsNoSql: MyNoSqlDbEntity
{
    public const string TableName = "myjetwallet-appversions";

    public enum Platforms
    {
        IOS = 0,
        Android =1
    }
    
    public static string GeneratePartitionKey() => "appVersions";
    public static string GenerateRowKey(Platforms platform) => platform.ToString();
    
    public uint MinVersion1 { get; set; }
    public uint MinVersion2 { get; set; }
    public uint MinVersion3 { get; set; }
    
    public uint RecommendVersion1 { get; set; }
    public uint RecommendVersion2 { get; set; }
    public uint RecommendVersion3 { get; set; }

    public static AppVersionsNoSql Create(Platforms platform, string minVersion, string recommendVersion)
    {
        var minVersionNum = ParseVersion(minVersion);
        var recommendVersionNum = ParseVersion(recommendVersion);

        var entity = new AppVersionsNoSql()
        {
            PartitionKey = GeneratePartitionKey(),
            RowKey = GenerateRowKey(platform),
            
            MinVersion1 = minVersionNum.Item1,
            MinVersion2 = minVersionNum.Item2,
            MinVersion3 = minVersionNum.Item3,
            
            RecommendVersion1 = recommendVersionNum.Item1,
            RecommendVersion2 = recommendVersionNum.Item2,
            RecommendVersion3 = recommendVersionNum.Item3
        };

        return entity;
    }

    public static (uint, uint, uint) ParseVersion(string version)
    {
        try
        {
            var prms = version.Split('.');
            if (prms.Length >= 3)
            {
                return (uint.Parse(prms[0]), uint.Parse(prms[1]), uint.Parse(prms[2]));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }

        throw new Exception($"Cannot parse version: {version}");
    }

    public bool IsNeedForceUpdate(string currentVersion)
    {
        var version = ParseVersion(currentVersion);

        if (MinVersion1 > version.Item1) return true;
        if (MinVersion1 < version.Item1) return false;
        
        
        if (MinVersion2 > version.Item2) return true;
        if (MinVersion2 < version.Item2) return false;
        
        if (MinVersion3 > version.Item3) return true;
        return false;
    }
    
    public bool IsNeedRecommendUpdate(string currentVersion)
    {
        var version = ParseVersion(currentVersion);

        if (RecommendVersion1 > version.Item1) return true;
        if (RecommendVersion1 < version.Item1) return false;
        
        
        if (RecommendVersion2 > version.Item2) return true;
        if (RecommendVersion2 < version.Item2) return false;
        
        if (RecommendVersion3 > version.Item3) return true;
        return false;
    }

    public string GetRecommendVersion() => $"{RecommendVersion1}.{RecommendVersion2}.{RecommendVersion3}";
    public string GetMinVersion() => $"{MinVersion1}.{MinVersion2}.{MinVersion3}";

}