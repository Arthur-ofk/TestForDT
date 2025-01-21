using Microsoft.Extensions.Configuration;
using TestForDT.DataBaseIntegration;
using TestForDT.DataProcesing;

namespace TestForDT
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            //reading setting file
            var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory()) 
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
            //connecting to db
            string connectionString = configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                Console.WriteLine("Connection string not found in appsettings.json");
                return;
            }
            var dbContext = new AppDbContext(connectionString);


            var filePaths = configuration.GetSection("FilePaths");
            string inputCsv = args.Length > 0 ? args[0] : filePaths["InputCsv"];
            string duplicatesCsv = args.Length > 1 ? args[1] : filePaths["DuplicatesCsv"];
            Console.WriteLine($"Input CSV Path: {inputCsv}");
            Console.WriteLine($"Duplicates CSV Path: {duplicatesCsv}");
            //reading file
            var csvProcessor = new CsvProcessor();
            var records = csvProcessor.ReadCsv(inputCsv);
            Console.WriteLine($"Total records read from CSV: {records.Count()}");
             var dataSaver = new DataProcessor(dbContext);
            await dataSaver.InsertRecordsAsync(records, duplicatesCsv);


            Console.WriteLine("process completed successfully.");
        } 
        
    }
}
