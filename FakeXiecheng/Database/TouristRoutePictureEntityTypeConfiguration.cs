using System.Collections.Generic;
using System.IO;
using System.Reflection;
using FakeXieCheng.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace FakeXieCheng.API.Database;

public class TouristRoutePictureEntityTypeConfiguration : IEntityTypeConfiguration<TouristRoutePicture>
{
    public void Configure(EntityTypeBuilder<TouristRoutePicture> builder)
    {
        var touristRoutePictureJsonData =
            File.ReadAllText(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) +
                             @"/Database/touristRoutePicturesMockData.json");
        IList<TouristRoutePicture> touristPRoutePictures =
            JsonConvert.DeserializeObject<IList<TouristRoutePicture>>(touristRoutePictureJsonData);
        if (touristPRoutePictures != null)
            builder.HasData(touristPRoutePictures);
    }
}