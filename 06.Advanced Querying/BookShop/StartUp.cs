namespace BookShop
{
    using BookShop.Models;
    using Data;
    using Initializer;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

    public class StartUp
    {
        public static void Main()
        {
            using var db = new BookShopContext();
            DbInitializer.ResetDatabase(db);

            string command = Console.ReadLine();

            //2
            //Console.WriteLine(GetBooksByAgeRestriction(db, command));

            //3
            //Console.WriteLine(GetGoldenBooks(db));

            //4
            //Console.WriteLine(GetBooksByPrice(db));

            //5
            //int year = int.Parse(Console.ReadLine());
            //Console.WriteLine(GetBooksNotReleasedIn(db, year));

            //6
            //Console.WriteLine(GetBooksByCategory(db, command));

            //7
            //Console.WriteLine(GetBooksReleasedBefore(db, command));

            //8
            //Console.WriteLine(GetAuthorNamesEndingIn(db, command));

            //9
            //Console.WriteLine(GetBookTitlesContaining(db, command));

            //10
            //Console.WriteLine(GetBooksByAuthor(db, command));

            //11
            //Console.WriteLine(CountBooks(db, int.Parse(command)));

            //12
            //Console.WriteLine(CountCopiesByAuthor(db));

            //13
            //Console.WriteLine(GetTotalProfitByCategory(db));

            //14
            //Console.WriteLine(GetMostRecentBooks(db));

            //15
            //IncreasePrices(db);

            //16
            Console.WriteLine(RemoveBooks(db));

        }

        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            StringBuilder output = new StringBuilder();

            var allBooks = context
                .Books
                .ToArray()
                .Where(b => b.AgeRestriction.ToString().ToLower() == command.ToLower())
                .Select(b => new
                {
                    b.Title
                })
                .OrderBy(b => b.Title)
                .ToArray();



            foreach (var b in allBooks)
            {
                output
                    .AppendLine($"{b.Title}");
            }


            return output.ToString().TrimEnd();
        }

        public static string GetGoldenBooks(BookShopContext context)
        {
            StringBuilder output = new StringBuilder();

            var allBooks = context
            .Books
                .ToArray()
                .Where(b => b.EditionType.ToString() == "Gold")
                .Where(b => b.Copies < 5000)
                .Select(b => new
                {
                    b.BookId,
                    b.Title
                })
                .OrderBy(b => b.BookId)
                .ToArray();



            foreach (var b in allBooks)
            {
                output
                    .AppendLine($"{b.Title}");
            }


            return output.ToString().TrimEnd();
        }

        public static string GetBooksByPrice(BookShopContext context)
        {
            StringBuilder output = new StringBuilder();

            var bookPrices = context
                .Books
                .Where(b => b.Price > 40)
                .OrderByDescending(b => b.Price)
                .Select(b => new
                {
                    b.Title,
                    b.Price
                })
                .ToArray();



            foreach (var book in bookPrices)
            {
                output
                    .AppendLine($"{book.Title} - ${book.Price:f2}");
            }


            return output.ToString().Trim();
        }

        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            StringBuilder output = new StringBuilder();

            var AllBooks = context
                .Books
                .Where(b => b.ReleaseDate.Value.Year != year)
                .OrderBy(b => b.BookId)
                .Select(b => new
                {
                    b.Title
                })
                .ToArray();



            foreach (var book in AllBooks)
            {
                output
                    .AppendLine($"{book.Title}");
            }


            return output.ToString().Trim();
        }

        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            StringBuilder output = new StringBuilder();
            string[] categories = input.ToLower().Split();

            var AllBooks = context
                .Books
                .Where(x => x.BookCategories.Any(x => categories.Contains(x.Category.Name.ToLower())))
                .Select(x => x.Title)
                .OrderBy(x => x)
                .ToArray();



            foreach (var book in AllBooks)
            {
                output
                    .AppendLine($"{book}");
            }


            return output.ToString().Trim();
        }

        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            StringBuilder output = new StringBuilder();
            DateTime time = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            var AllBooks = context
                .Books
                .Where(x => x.ReleaseDate < time)
                .OrderByDescending(x => x.ReleaseDate)
                .Select(x => new
                {
                    x.Title,
                    EditionType = x.EditionType.ToString(),
                    x.Price
                })
                .ToArray();



            foreach (var book in AllBooks)
            {
                output
                    .AppendLine($"{book.Title} - {book.EditionType} - ${book.Price:f2}");
            }


            return output.ToString().Trim();
        }

        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            StringBuilder output = new StringBuilder();

            var authorNames = context.Authors
                .Where(x => x.FirstName.EndsWith(input))
                .Select(x => new
                {
                    FullName = $"{x.FirstName} {x.LastName}"
                })
                .OrderBy(x => x.FullName)
                .ToArray();



            foreach (var a in authorNames)
            {
                output
                    .AppendLine($"{a.FullName}");
            }


            return output.ToString().Trim();
        }

        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            StringBuilder output = new StringBuilder();

            var booksTitles = context
                .Books
                .Where(x => x.Title.ToLower().Contains(input.ToLower()))
                .Select(x => new
                {
                    x.Title
                })
                .OrderBy(x => x.Title)
                .ToArray();



            foreach (var b in booksTitles)
            {
                output
                    .AppendLine($"{b.Title}");
            }


            return output.ToString().Trim();
        }

        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            StringBuilder output = new StringBuilder();

            var booksTitles = context
                .Books
                .Where(x => x.Author.LastName.ToLower().StartsWith(input.ToLower()))
                .OrderBy(x => x.BookId)
                .Select(x => new
                {
                    x.Title,
                    AuthorName = $"{x.Author.FirstName} {x.Author.LastName}"
                })
                .ToArray();

            foreach (var b in booksTitles)
            {
                output
                    .AppendLine($"{b.Title} ({b.AuthorName})");
            }


            return output.ToString().Trim();
        }

        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            StringBuilder output = new StringBuilder();

            int bookCount = context
                .Books
                .Where(x => x.Title.Length > lengthCheck)
                .OrderBy(x => x.BookId)
                .Count();

            return bookCount;
        }

        public static string CountCopiesByAuthor(BookShopContext context)
        {
            StringBuilder output = new StringBuilder();

            var copiesOp = context
                .Authors
                .Select(x => new
                {
                    Fullname = $"{x.FirstName} {x.LastName}",
                    Count = x.Books.Sum(b => b.Copies)
                })
                .OrderByDescending(x => x.Count)
                .ToArray();

            foreach (var b in copiesOp)
            {
                output
                    .AppendLine($"{b.Fullname} - {b.Count}");
            }


            return output.ToString().Trim();
        }

        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            StringBuilder output = new StringBuilder();

            var categoriesAll = context
                .Categories
                .Select(c => new
                {
                    c.Name,
                    Profit = c.CategoryBooks.Sum(cb => cb.Book.Copies * cb.Book.Price) 
                })
                .OrderByDescending(c => c.Profit)
                .ToArray();

            foreach (var c in categoriesAll)
            {
                output
                    .AppendLine($"{c.Name} ${c.Profit:f2}");
            }


            return output.ToString().Trim();
        }

        public static string GetMostRecentBooks(BookShopContext context)
        {
            StringBuilder output = new StringBuilder();

            var categoriesAll = context
                .Categories
                .Select(c => new
                {
                    c.Name,
                    top3 = c.CategoryBooks
                    .OrderByDescending(cb => cb.Book.ReleaseDate)
                    .Take(3)
                    .Select(t => new
                    {
                        title = t.Book.Title,
                        year = t.Book.ReleaseDate.Value.Year
                    })
                    .ToArray()
                })
                .OrderBy(c => c.Name)
                .ToArray();

            foreach (var c in categoriesAll)
            {
                output
                    .AppendLine($"--{c.Name}");

                foreach (var t in c.top3)
                {
                    output
                    .AppendLine($"{t.title} ({t.year})");
                }
            }


            return output.ToString().Trim();
        }

        public static void IncreasePrices(BookShopContext context)
        {
            int yearLessThan = 2010;

            IQueryable<Book> booksToUpdate = context.Books
                .Where(x => x.ReleaseDate.Value.Year < yearLessThan);

            int priceToIncrease = 5;
            foreach (Book book in booksToUpdate)
            {
                book.Price += priceToIncrease;
            }

            context.SaveChanges();
        }

        public static int RemoveBooks(BookShopContext context)
        {
            int copiesLessThan = 4200;

            IQueryable<Book> booksToDelete = context.Books
                .Where(x => x.Copies < copiesLessThan);

            int deletedBooks = booksToDelete.Count();

            context.Books.RemoveRange(booksToDelete);
            context.SaveChanges();

            return deletedBooks;
        }
    }
}
