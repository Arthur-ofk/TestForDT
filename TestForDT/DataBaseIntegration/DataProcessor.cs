using CsvHelper.Configuration;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestForDT.Entities;
using Microsoft.EntityFrameworkCore;
using TimeZoneConverter;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace TestForDT.DataBaseIntegration
{
    public class DataProcessor
    {
        private readonly AppDbContext _context;
        public DataProcessor(AppDbContext context)
        {
            _context = context;
        }
        public async Task InsertRecordsAsync(IEnumerable<TripRecord> records,string duplicatesFilePath)
        {
            //removing duplicates
            var uniqueRecords = await RemoveDuplicatesAsync(records, duplicatesFilePath);
            if (uniqueRecords == null || !uniqueRecords.Any())
            {
                Console.WriteLine("No records to insert(Probably source file was empty)");
                return;
            }
            //transforming data before writing into DB
            var formattedRecords = uniqueRecords.Select(record =>
            {
                record.PickupDateTime = ConvertToUtc(record.PickupDateTime);
                record.DropoffDateTime = ConvertToUtc(record.DropoffDateTime);
                record.StoreAndFwdFlag = record.StoreAndFwdFlag?.Trim() switch
                {
                    "N" => "No",
                    "Y" => "Yes",
                    _ => record.StoreAndFwdFlag
                };

                return record;
            }).ToList();

            await _context.TripRecords.AddRangeAsync(formattedRecords);
            await _context.SaveChangesAsync();
            Console.WriteLine($"{formattedRecords.Count()} records inserted into the database.");

        }
        public async Task<IEnumerable<TripRecord>> RemoveDuplicatesAsync(IEnumerable<TripRecord> records, string duplicatesFilePath)
        {
            return await Task.Run(() =>
            {
                //Group records by unique fields
                var groupedRecords = records
                .GroupBy(record => new {
                    PickupDateTime = record.PickupDateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    DropoffDateTime = record.DropoffDateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    PassengerCount = record.PassengerCount
                });

                //Find duplicates
                var duplicates = groupedRecords
               .Where(group => group.Count() > 1)
               .SelectMany(group => group.Skip(1))
               .ToList();

                //saving duplicates
                if (duplicates.Any())
                {
                    Console.WriteLine($"Found {duplicates.Count} duplicates.");
                    using var writer = new StreamWriter(duplicatesFilePath);
                    using var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture));
                    csv.WriteRecords(duplicates);
                    Console.WriteLine($"Duplicates saved to: {duplicatesFilePath}");
                }
                //return only unique records
                return groupedRecords.Select(group => group.First());
            });
        }
        private DateTime ConvertToUtc(DateTime dateTime)
        {
            // find timezone for  EST
            var estTimeZone = TZConvert.GetTimeZoneInfo("Eastern Standard Time");

            // convert from EST to UTC
            return TimeZoneInfo.ConvertTimeToUtc(dateTime, estTimeZone);
        } 
    }
}
