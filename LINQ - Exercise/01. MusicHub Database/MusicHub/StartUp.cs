namespace MusicHub
{
    using System;

    using Data;
    using Initializer;

    public class StartUp
    {
        public static void Main(string[] args)
        {
            MusicHubDbContext context = 
                new MusicHubDbContext();

            DbInitializer.ResetDatabase(context);

           var result = ExportAlbumsInfo(context, 9);
            Console.WriteLine(result);
        }

        public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
        {
            StringBuilder sb = new StringBuilder();

            var albumInfo = context.Producers
                .FirstOrDefault(producer => producer.Id == producerId)
                .Albums
                .Select(a => new
                {
                  albumName=  a.Name,
                  relaseDate = a.ReleaseDate.ToString("MM/dd/yyyy",CultureInfo.InvariantCulture),
                  producerName= a.Producer.Name,
                  Songs=a.Songs.Select(s=>new
                  {
                     songName = s.Name,
                     price = s.Price,
                     songWriterName =s.Writer.Name
                  }).OrderByDescending(x=>x.songName)
                      .ThenBy(x=>x.songWriterName)
                      .ToList(),
                  albumPrice =a.Price
                }).OrderByDescending(x=>x.albumPrice).ToList();
            
            foreach (var item in albumInfo)
            {
                int counter = 1;
                sb.AppendLine($"-AlbumName: {item.albumName}");
                sb.AppendLine($"-ReleaseDate: {item.relaseDate}");
                sb.AppendLine($"-ProducerName: {item.producerName}");
                sb.AppendLine($"-Songs:");
                foreach (var song in item.Songs)
                {
                    sb.AppendLine($"---#{counter++}");
                    sb.AppendLine($"---SongName: {song.songName}");
                    sb.AppendLine($"---Price: {song.price:f2}");
                    sb.AppendLine($"---Writer: {song.songWriterName}");
                }

                sb.AppendLine($"-AlbumPrice: {item.albumPrice:f2}");
            }
            return sb.ToString().TrimEnd();
        }

        public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
        {
            throw new NotImplementedException();
        }
    }
}
