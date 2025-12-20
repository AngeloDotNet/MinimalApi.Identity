namespace MinimalApi.Identity.Core.Database.Configurations;

//public class FeatureFlagConfiguration : IEntityTypeConfiguration<FeatureFlag>
//{
//    public void Configure(EntityTypeBuilder<FeatureFlag> builder)
//    {
//        var featureFlagList = new List<FeatureFlag>();
//        var idRiga = 1;

//        foreach (var ff in Enum.GetValues<FeatureFlagType>())
//        {
//            featureFlagList.Add(new FeatureFlag
//            {
//                Id = idRiga,
//                Name = ff.ToString(),
//                IsEnabled = false
//            });

//            idRiga++;
//        }

//        builder.HasData(featureFlagList);
//    }
//}