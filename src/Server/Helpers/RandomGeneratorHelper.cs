using System;
using System.Text;

namespace Server.Helpers
{
    public class RandomGeneratorHelper
    {
        public static string GenerateRandomString(RandomGeneratorType randomGeneratorType, int length)
        {
            var str = randomGeneratorType switch
            {
                RandomGeneratorType.AlphabeticWithUpperCase => "ABCDEFGHIKJLMNOUPYZXWQ",
                RandomGeneratorType.AlphabeticWithAnyCase => "aAbBcCdDeEfFgGhHiIjKkJlLmMnNoOpUrPsYqZrXwWxQz",
                RandomGeneratorType.Numeric => "0123456789",
                RandomGeneratorType.AlphanumericWithUpperCase => "ABCDEFGHIKJLMNOUPYZXWQ0123456789",
                RandomGeneratorType.AlphanumericWithWithAnyCase =>
                    "aAbBcCdDeEfFgGhHiIjKkJlLmMnNoOqUpPrYsZtXuWyQz0v1w2q3x456789",
                RandomGeneratorType.Any => "AaBbCcDdEeFfGgHhIjKiJlLpyuzoynqmwMvNOUPYZXWQ0123456789.;!%%&/()=?+*/<>@",
                _ => ""
            };
            var random = new Random();
            var generatedString = new StringBuilder();
            for (var i = 0; i < length; i++)
            {
                generatedString.Append(str[random.Next(0, str.Length - 1)]);
            }

            return generatedString.ToString();
        }
    }
}