public class AccountNumberGenerator
{
    private readonly string[] prefixes;
    public AccountNumberGenerator()
    {
        prefixes = GeneratePrefixes();
    }
    private string[] GeneratePrefixes()
    {
        var list = new List<string>();
        for (char first = 'A'; first <= 'Z'; first++)
        {
            for (char second = 'A'; second <= 'Z'; second++)
            {
                list.Add($"{first}{second}");
            }
        }
        return list.ToArray();
    }

    public string GenerateNextAccountNumber(string lastAccountNumber)
    {
        string datePart = DateTime.Now.ToString("ddMMyy");
        const string fixedPrefix = "ACC";
        if (string.IsNullOrEmpty(lastAccountNumber))
        {
            return $"{fixedPrefix}{datePart}AA0001";
        }
        if (lastAccountNumber.Length != 15)
        {
            throw new ArgumentException("Invalid account number length. Expected 15 characters.");
        }
        string lastDatePart = lastAccountNumber.Substring(3, 6);
        string lastPrefix = lastAccountNumber.Substring(9, 2);
        string lastNumberPart = lastAccountNumber.Substring(11, 4);
        int lastNumber = int.Parse(lastNumberPart);
        if (lastDatePart != datePart)
        {
            return $"{fixedPrefix}{datePart}AA0001";
        }
        if (lastNumber < 9999)
        {
            int newNumber = lastNumber + 1;
            return $"{fixedPrefix}{datePart}{lastPrefix}{newNumber:D4}";
        }
        else
        {
            int prefixIndex = Array.IndexOf(prefixes, lastPrefix);
            if (prefixIndex == -1)
            {
                throw new ArgumentException("Invalid prefix in account number.");
            }
            if (prefixIndex == prefixes.Length - 1)
            {
                throw new InvalidOperationException("No more prefixes available for account number generation.");
            }
            string newPrefix = prefixes[prefixIndex + 1];
            return $"{fixedPrefix}{datePart}{newPrefix}0001";
        }
    }
}