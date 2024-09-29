using Bogus;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace FakeUserDataGenerator.Services
{
    public class UserDataService
    {
        public IEnumerable<UserRecord> GenerateUserRecords(string region, int seed, int errorCount, int page, int pageSize)
        {
            var faker = GetFakerByRegion(region);
            Random random = new Random(seed + page);

            var records = new List<UserRecord>();

            for (int i = 0; i < pageSize; i++)
            {
                var userRecord = new UserRecord
                {
                    Index = page * pageSize + i + 1,
                    Identifier = Guid.NewGuid().ToString(),
                    Name = faker.Name.FullName(),
                    Address = faker.Address.FullAddress(),
                    Phone = faker.Phone.PhoneNumber()
                };

                //Inject errors if errorCount > 0
                if (errorCount > 0)
                    {
                        userRecord.Name = InjectError(userRecord.Name, errorCount, random);
                        userRecord.Address = InjectError(userRecord.Address, errorCount, random);
                        userRecord.Phone = InjectError(userRecord.Phone, errorCount, random);
                    }

                records.Add(userRecord);
            }
            return records;
        }

        private Faker GetFakerByRegion(string region)
        {
            var faker = new Faker();

            switch (region.ToLower())
            {
                case "poland":
                    faker.Locale = "pl";
                    break;
                case "usa":
                    faker.Locale = "en";
                    break;
                case "georgia":
                    faker.Locale = "ka";
                    break;
                default:
                    faker.Locale = "en";
                    break;
            }

            return faker;
        }

        private string InjectError(string input, int errorCount, Random random)
        {
            var chars = input.ToCharArray();
            for (int i = 0; i < errorCount; i++)
            {
                int errorType = random.Next(0, 3);
                switch (errorType)
                {
                    case 0:
                        // Delete character
                        if (chars.Length > 1)
                        {
                            int pos = random.Next(0, chars.Length);
                            input = input.Remove(pos, 0);
                        }
                        break;
                    case 1:
                        // Add random character
                        int insertPos = random.Next(0, chars.Length);
                        char randomChar = (char)random.Next(97, 122); // random lowercase letter
                        input = input.Insert(insertPos, randomChar.ToString());
                        break;
                    case 2:
                        // Swap adjacent characters
                        if (chars.Length > 1)
                        {
                            int swapPos = random.Next(0, chars.Length - 1);
                            char temp = chars[swapPos];
                            chars[swapPos] = chars[swapPos + 1];
                            chars[swapPos + 1] = temp;
                            input = new string(chars);
                        }
                        break;
                }
            }
            return input;
        }
    }

    public class UserRecord
    {
        public int Index { get; set; }
        public string Identifier { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
    }
}
