using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.EntityFrameworkCore;
namespace LINQtoObject
{
    class Program
    {
        
        public class Context : DbContext
        {
            public DbSet<Author> Authors { get; set; }
            public DbSet<Book> Books { get; set; }
            public DbSet<Publisher> Publishers { get; set; }
            public DbSet<Subject> Subjects { get; set; }
            public DbSet<Review> Reviews { get; set; }
            public DbSet<User> Users { get; set; }
            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseSqlServer(@"Server=.;Database=BookStore;Integrated Security=True;TrustServerCertificate=True;MultipleActiveResultSets=True;")
                .LogTo(log => Debug.WriteLine(log));

            }

        }
        static void Main(string[] args)
        {
            Context context = new Context();
            foreach (var Author in SampleData.Authors)
            {
                context.Authors.Add(Author);
            }
            foreach (var book in SampleData.Books)
            {
                context.Books.Add(book);
            }
            foreach (var publisher in SampleData.Publishers)
            {
                context.Publishers.Add(publisher);
            }
            foreach (var subject in SampleData.Subjects)
            {
                context.Subjects.Add(subject);
            }
            context.SaveChanges();
         

            // 1- Display book title and its ISBN.
            var bookInfo =  context.Books.Select(book => new { title = book.Title , isbn = book.Isbn});
            foreach (var title in bookInfo)
            {
                Console.WriteLine(title.title);
                Console.WriteLine(title.isbn);
            
            }
            Console.WriteLine("---------------------------------");
            // 2- Display the first 3 books with price more than 25.
            IEnumerable<Book> list2 = context.Books
                .Where(book => book.Price > 25)
                .Take(3);
            foreach (Book book in list2)
            {
                Console.WriteLine(book.ToString());
            }
            Console.WriteLine("---------------------------------");
            // 3- Display Book title along with its publisher name.
            var list3 = context.Books.Select(book => new { title = book.Title, Publisher = book.Publisher.Name });
            foreach (var book in list3)
            {
                Console.WriteLine($"book title = {book.title} publisher = {book.Publisher}");
            }
            Console.WriteLine("---------------------------------");
            // 4- Find the number of books which cost more than 20.
            int NumberOfBookMoreThan20 = context.Books.
                Where(book => book.Price > 20)
                .Count();
            Console.WriteLine(NumberOfBookMoreThan20);
            Console.WriteLine("---------------------------------");

            // 5- Display book title, price and subject name sorted by its subject name ascending and by its price descending
            var list4 = context.Books
                .Select(book => new { title = book.Title, price = book.Price, subject = book.Subject.Name })
                .OrderBy(book => book.subject)
                .ThenByDescending(book => book.price);
            foreach (var book in list4)
            {
                Console.WriteLine($"book title = {book.title} price = {book.price} subject = {book.subject}");
            }
            Console.WriteLine("---------------------------------");

            // 6- Display All subjects with books related to this subject. (Using 2 methods).
            var list5 = context.Books
                .Select(book => new { title = book.Title, subject = book.Subject.Name })
                .GroupBy(book => book.subject);
            foreach (var book in list5)
            {
                Console.WriteLine($"subject = {book.Key}");
                foreach (var b in book)
                {
                    Console.WriteLine($"book title = {b.title}");
                }
            }
            Console.WriteLine("---------------------------------");
/*
            var list6 = context.GetBooks().OfType<Book>().Select(book => new { title = book.Title, price = book.Price });
            foreach (var book in list6)
            {
                Console.WriteLine($"book title = {book.title} price = {book.price}");
            }
            Console.WriteLine("---------------------------------");
*/
            var bookGroup = context.Books
                .Select(book => book)
                .GroupBy(book => new {Pname = book.Publisher.Name, Sname =  book.Subject.Name});
            foreach (var book in bookGroup) {     
                Console.WriteLine($"Publisher = {book.Key.Pname} Subject = {book.Key.Sname}");
                foreach (var b in book)
                {
                    Console.WriteLine($"book title = {b.Title}");
                }
            }
            Console.WriteLine("---------------------------------");
            
            Console.WriteLine("Enter Publisher Name");
            string publisherName = Console.ReadLine()?.ToLower();
            Console.WriteLine("Enter sorting criteria : Title, Price, PageCount, PublicationDate");
            string sortingCriteria = Console.ReadLine();
            Console.WriteLine("Enter sorting order : Ascending, Descending just Enter ASC or DESC");
            string sortingOrder = Console.ReadLine()?.ToLower();
            var list7 = context.Books.Where(book => book.Publisher.Name.ToLower() == publisherName);
            var sortedList7 = sortingOrder == "ASC" ? list7.OrderBy(book => book.GetType().GetProperty(sortingCriteria).GetValue(book, null))
                : list7.OrderByDescending(book => book.GetType().GetProperty(sortingCriteria).GetValue(book, null));

            var finalList7 = sortedList7.Select(book => new { title = book.Title});
           
            foreach (var book in finalList7)
            {
                Console.WriteLine($"book title = {book.title}");
            }
            Console.ReadLine();
        }
    }
}
