using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using BookShop.Models.Enums;



namespace BookShop
{
    using Data;
    using Initializer;

    public class StartUp
    {
        public static void Main()
        {
            using var db = new BookShopContext();
            DbInitializer.ResetDatabase(db);


           
        }
        //Problem: 1
        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {


            var bookTitle = context.Books.AsEnumerable()
                .Where(x => x.AgeRestriction.ToString().ToLower() == command.ToLower())
                .Select(bt => bt.Title).OrderBy(bt => bt).ToList();

            return String.Join(Environment.NewLine, bookTitle);
        }
        //Problem: 2
        public static string GetGoldenBooks(BookShopContext context)
        {

            var books = context.Books
                .Where(x => x.EditionType == EditionType.Gold && x.Copies < 5000)
                .OrderBy(x => x.BookId)
                .Select(x => x.Title)
                .ToList();

            return String.Join(Environment.NewLine, books);
        }
        //Problem: 3
        public static string GetBooksByPrice(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();
            var result = context.Books
                .Select(b => new
                {
                    bookTitle = b.Title,
                    bookPrice = b.Price
                })
                .Where(b => b.bookPrice > 40)
                .OrderByDescending(b => b.bookPrice)
                .ToList();

            foreach (var book in result)
            {
                sb.AppendLine($"{book.bookTitle} - " +
                              $"${book.bookPrice:f2}");
            }

            return sb.ToString().TrimEnd();
        }
        //Problem: 4
        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            StringBuilder sb = new StringBuilder();
            var books = context.Books.Select(x => new
            {
                titleBook = x.Title,
                date = x.ReleaseDate,
                bookId = x.BookId
            })
                .Where(x => x.date.Value.Year != year).OrderBy(x => x.bookId).ToList();
            foreach (var book in books)
            {
                sb.AppendLine(book.titleBook);
            }
            return sb.ToString().TrimEnd();
        }
        //Problem: 5
        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            var categories = input.Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(x => x.ToLower()).ToArray();

            var sb = new StringBuilder();
            List<string> bookCategory = new List<string>();
            foreach (var item in categories)
            {
                List<string> current = context.Books
                    .Where(b => b.BookCategories
                        .Any(bc => bc.Category.Name.ToLower() == item))
                    .Select(t => t.Title).ToList();

                bookCategory.AddRange(current);
            }

            bookCategory = bookCategory.OrderBy(x => x).ToList();

            return String.Join(Environment.NewLine, bookCategory);
        }
        //Problem: 6
        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            var parserDate = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            StringBuilder sb = new StringBuilder();

            var books = context.Books
                .Where(b => b.ReleaseDate < parserDate)
                .OrderByDescending(x => x.ReleaseDate)
                .Select(x => new
                {
                    title = x.Title,
                    editionalType = x.EditionType,
                    price = x.Price
                }).ToList();

            foreach (var book in books)
            {
                sb.AppendLine($"{book.title} - " +
                              $"{book.editionalType} - " +
                              $"${book.price:f2}");
            }
            return sb.ToString().TrimEnd();
        }
        //Problem: 7
        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            StringBuilder sb = new StringBuilder();

            var authors = context.Authors
                .Where(a => a.FirstName.EndsWith(input))
                .Select(x => new
                {
                    fullName = string.Concat(x.FirstName, " ", x.LastName)

                }).ToList();

            authors = authors.OrderBy(x => x.fullName).ToList();
            foreach (var item in authors)
            {
                sb.AppendLine(item.fullName);
            }

            return sb.ToString().TrimEnd();
        }
        //Problem: 8
        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            var titleOfBook = context.Books
                .Where(b => b.Title.ToLower().Contains(input.ToLower()))
                .Select(x => x.Title).ToList();
            titleOfBook = titleOfBook.OrderBy(x => x).ToList();
            return String.Join(Environment.NewLine, titleOfBook);
        }
        //Problem: 9
        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            StringBuilder sb = new StringBuilder();

            var titleOfBooks = context.Books
                .Where(x => x.Author.LastName.ToLower().StartsWith(input.ToLower()))
                .Select(x => new
                {
                    fullName = string.Concat(x.Author.FirstName, " ", x.Author.LastName),
                    title = x.Title,
                    bookId = x.BookId
                }).OrderBy(x => x.bookId).ToList();

            foreach (var title in titleOfBooks)
            {
                sb.AppendLine($"{title.title} ({title.fullName})");
            }
            return sb.ToString().TrimEnd();
        }
        //Problem: 10
        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            var result = context.Books.Where(x => x.Title.Length > lengthCheck).ToList();
            return result.Count;
        }
        //Problem: 11
        public static string CountCopiesByAuthor(BookShopContext context)
        {
            var sb = new StringBuilder();

            var authorBooks = context
                .Authors
                .Select(a => new
                {
                    FirstName = a.FirstName,
                    LastName = a.LastName,
                    BooksCount = a.Books
                        .Sum(b => b.Copies)
                })
                .OrderByDescending(b => b.BooksCount)
                .ToList();

            foreach (var author in authorBooks)
            {
                sb.AppendLine($"{author.FirstName} " +
                              $"{author.LastName} - " +
                              $"{author.BooksCount}");
            }

            return sb.ToString().Trim();
        }
        //Problem: 12
        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var results = context.Categories
                .Select(x => new
                {

                    categoryName = x.Name,
                    proffit = x.CategoryBooks.Sum(cb => cb.Book.Copies * cb.Book.Price)

                })
                .OrderByDescending(x => x.proffit)
                .ThenBy(x => x.categoryName)
                .ToList();
            foreach (var itemResult in results)
            {
                sb.AppendLine($"{itemResult.categoryName} " +
                              $"${itemResult.proffit:f2}");
            }

            return sb.ToString().TrimEnd();
        }
        //Problem: 13
        public static string GetMostRecentBooks(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();
            var categories = context.Categories.Select(c => new
            {
                Name = c.Name,
                Books = c.CategoryBooks.Select(cb => new
                {
                    title = cb.Book.Title,
                    releaseDate = cb.Book.ReleaseDate
                })
                      .OrderByDescending(cb => cb.releaseDate)
                      .Take(3)
                      .ToList()
            })
              .OrderBy(c => c.Name)
              .ToList();
            foreach (var category in categories)
            {
                sb.AppendLine($"--{category.Name}");
                foreach (var book in category.Books)
                {
                    sb.AppendLine($"{book.title}" + $" ({book.releaseDate.Value.Year})");
                }
            }
            return sb.ToString().TrimEnd();
        }
        //Problem: 14
        public static void IncreasePrices(BookShopContext context)
        {
            var pricesBooks = context.Books.Where(x => x.ReleaseDate.Value.Year < 2010).ToList();
            foreach (var prices in pricesBooks)
            {
                prices.Price += 5;
            }

            context.SaveChanges();
        }
        //Problem: 15
        public static int RemoveBooks(BookShopContext context)
        {
            var removeBooks = context.Books.Where(x => x.Copies < 4200).ToList();

            var deletedFields = removeBooks.Count;

            var bookCategories = context.BooksCategories.Where(x => x.Book.Copies < 4200);

            context.BooksCategories.RemoveRange(bookCategories);
            context.Books.RemoveRange(removeBooks);

            context.SaveChanges();

            return deletedFields;
        }
    }
}
