bool useExampleInput = false;

(string inputFilenamePartA, string inputFilenamePartB) = useExampleInput
	? ("exampleInputA.txt", "exampleInputB.txt")
	: ("input.txt", "input.txt");

long resultPartA = File
	.ReadAllLines(inputFilenamePartA)
	.Select(long.Parse)
	.Sum(item => CalculateSecretNumbers(item, 2000)[^1]);

var priceLookupForChangeAndInput = File
	.ReadAllLines(inputFilenamePartB)
	.AsParallel()
	.Select(long.Parse)
	.Select(item => CalculateSecretNumbers(item, 2000))
	.Select(secretsForAnInput => Enumerable
		.Range(4, secretsForAnInput.Count - 4)
		.Select(index =>
			(Price: (int)(secretsForAnInput[index] % 10),
				Changes: new PriceChanges(
					(int)(secretsForAnInput[index - 3] % 10 - secretsForAnInput[index - 4] % 10),
					(int)(secretsForAnInput[index - 2] % 10 - secretsForAnInput[index - 3] % 10),
					(int)(secretsForAnInput[index - 1] % 10 - secretsForAnInput[index - 2] % 10),
					(int)(secretsForAnInput[index] % 10 - secretsForAnInput[index - 1] % 10)))));
Dictionary<PriceChanges, int> sumOfPriceForChanges = new();
foreach (IEnumerable<(int Price, PriceChanges Changes)> pricesAndChangesForInput in priceLookupForChangeAndInput)
{
	HashSet<PriceChanges> seenChanges = new();
	foreach ((int Price, PriceChanges Changes) priceAndChanges in pricesAndChangesForInput)
	{
		if (seenChanges.Add(priceAndChanges.Changes))
			sumOfPriceForChanges[priceAndChanges.Changes] =
				sumOfPriceForChanges.GetValueOrDefault(priceAndChanges.Changes) + priceAndChanges.Price;
	}
}

int resultPartB = sumOfPriceForChanges.Values.Max();

Console.WriteLine("Day 22 - Monkey Market");
Console.WriteLine($"A: The sum of the 2000th secret numbers is {resultPartA}."); // 37327623, 15613157363
Console.WriteLine($"B: The maximum number of bananas one can get is {resultPartB}."); // 23, 1784

List<long> CalculateSecretNumbers(long secretNumber, int numberOfSecretsToCalculate)
{
	List<long> secretNumbers = new List<long>(numberOfSecretsToCalculate) { CalculateNextSecretNumber(secretNumber) };
	for (int i = 1; i < numberOfSecretsToCalculate; ++i)
		secretNumbers.Add(CalculateNextSecretNumber(secretNumbers[^1]));

	return secretNumbers;
}

long CalculateNextSecretNumber(long secretNumber)
{
	secretNumber ^= (secretNumber << 6) & 0xFFFFFF;
	secretNumber ^= (secretNumber >> 5) & 0xFFFFFF;
	return (secretNumber ^ secretNumber << 11) & 0xFFFFFF;
}

record PriceChanges(int ChangeMinusThree, int ChangeMinusTwo, int ChangeMinusOne, int CurrentChange);
