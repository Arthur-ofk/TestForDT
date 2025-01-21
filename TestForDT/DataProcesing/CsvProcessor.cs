using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestForDT.DataProcesing.Mapping;
using TestForDT.Entities;

namespace TestForDT.DataProcesing
{
    public  class CsvProcessor
    {
       public IEnumerable<TripRecord> ReadCsv(string filepath)
        {
            //create config for reader
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                PrepareHeaderForMatch = args => args.Header.ToLower(),
                MissingFieldFound = null,
            };
            //read file
            var reader = new StreamReader(filepath);
            var csv = new CsvReader(reader, config);
            csv.Context.RegisterClassMap<TripRecordMap>();
            return csv.GetRecords<TripRecord>().ToList();
            
        }

    }
}
