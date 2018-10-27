namespace Rnwood.SmtpServer
{
    using System;

    public class RandomIntegerGenerator : IRandomIntegerGenerator
    {
        private static Random random = new Random();

        public int GenerateRandomInteger(int minValue, int maxValue)
        {
            return random.Next(minValue, maxValue);
        }
    }
}