using CsvHelper;
using DataAccessLayer.DataContext;
using DataAccessLayer.Entity;
using DataAccessLayer.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository
{
    public class BookRepository:IBookRepository

    {

        private readonly  ApplicationDbContext _context;

        public BookRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        /*
                public string LoadBooksFromCsv()
                {
                    try
                    {
                        string basePath = AppDomain.CurrentDomain.BaseDirectory;  //gets basedirectory where applicaton running 
                        string relativePath = @"..\..\..\..\DataAccessLayer\Csv\books.csv";
                        string fullPath = Path.GetFullPath(Path.Combine(basePath, relativePath));

                        var config = new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
                        {
                            HeaderValidated = null,         // Prevent header mismatch exceptions
                            MissingFieldFound = null        // Ignore missing fields
                        };

                        using (var reader = new StreamReader(fullPath))
                        using (var csv = new CsvReader(reader, config))
                        {
                            var records = csv.GetRecords<Book>().ToList();
                            _context.Books.AddRange(records);
                            _context.SaveChanges();
                            return "Books saved successfully from CSV.";
                        }
                    }
                    catch (Exception ex)
                    {
                        return $"Error occurred while saving books: {ex.Message}";
                    }
                }
                */

        public string LoadBooksFromCsv()
        {
            try
            {
                string basePath = AppDomain.CurrentDomain.BaseDirectory;
                string relativePath = @"..\..\..\..\DataAccessLayer\Csv\books.csv";
                string fullPath = Path.GetFullPath(Path.Combine(basePath, relativePath));

                var config = new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HeaderValidated = null,
                    MissingFieldFound = null
                };

                using (var reader = new StreamReader(fullPath))
                using (var csv = new CsvReader(reader, config))
                {
                    var records = csv.GetRecords<Book>().ToList();

                    // Set Id = 0 and timestamps so EF will treat as new inserts
                    foreach (var book in records)
                    {
                        book.Id = 0; // Ignored by EF; auto-incremented
                        book.CreatedAt = DateTime.Now;
                        book.UpdatedAt = DateTime.Now;
                    }

                    _context.Books.AddRange(records);
                    _context.SaveChanges();

                    return "Books saved successfully from CSV.";
                }
            }
            catch (Exception ex)
            {
                return $"Error occurred while saving books: {ex.Message}";
            }
        }



    }
}
