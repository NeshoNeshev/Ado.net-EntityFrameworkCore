using System.Collections.Generic;
using BookShop.Data.Models.Enums;
using BookShop.DataProcessor.ExportDto;

namespace BookShop.DataProcessor
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using Data;
    using Newtonsoft.Json;
    using Formatting = Newtonsoft.Json.Formatting;

    public class Serializer
    {
        public static string ExportMostCraziestAuthors(BookShopContext context)
        {
            var authors = context.Authors
                .ToList()
                .Select(a => new
                {
                    AuthorName = a.FirstName + " " + a.LastName,
                    Books = a.AuthorsBooks.OrderByDescending(y => y.Book.Price)
                        .Select(x => new
                        {
                            BookName = x.Book.Name,
                            BookPrice = x.Book.Price.ToString("f2")

                        }).ToList()

                })
                .OrderByDescending(x => x.Books.Count())
                .ThenBy(x => x.AuthorName).ToList();

            var output = JsonConvert.SerializeObject(authors, Formatting.Indented);
            return output;
        }

        public static string ExportOldestBooks(BookShopContext context, DateTime date)
        {
            List<ExportOldestBook> booksDtos = context.Books
                .Where(x => x.PublishedOn < date && x.Genre == Genre.Science)
                .ToList()
                .OrderByDescending(x => x.Pages)
                .ThenByDescending(x => x.PublishedOn)
                .Take(10)
                .Select(x => new ExportOldestBook()
                {
                    Date = x.PublishedOn.ToString("d", CultureInfo.InvariantCulture),
                    Name = x.Name,
                    Pages = x.Pages
                })
                .ToList();

            StringBuilder sb = new StringBuilder();
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(String.Empty, String.Empty);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<ExportOldestBook>), new XmlRootAttribute("Books"));
            using (StringWriter writer = new StringWriter(sb))
            {
                xmlSerializer.Serialize(writer, booksDtos, namespaces);
            }

            return sb.ToString().TrimEnd();
        }
    }
}