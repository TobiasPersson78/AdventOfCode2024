using System.Text.RegularExpressions;

bool useExampleInput = false;

(string inputFilenamePartA, string inputFilenamePartB) = useExampleInput
	? ("exampleInputA.txt", "exampleInputB.txt")
	: ("input.txt", "input.txt");

Func<string, bool, int> calculateProductSum = (input, ignoreDoDont) => Regex
	.Matches(input, @"do\(\)|don't\(\)|mul\((\d+),(\d+)\)")
	.Aggregate(
		(AddProducts: true, ProductSum: 0),
		(acc, curr) => curr.Groups[0].Value switch
		{
			"do()" => (true, acc.ProductSum),
			"don't()" => (ignoreDoDont, acc.ProductSum),
			_ => acc.AddProducts
				? (true, acc.ProductSum + int.Parse(curr.Groups[1].Value) * int.Parse(curr.Groups[2].Value))
				: acc
		})
	.ProductSum;
int productSumA = calculateProductSum(File.ReadAllText(inputFilenamePartA), true);
int productSumB = calculateProductSum(File.ReadAllText(inputFilenamePartB), false);

Console.WriteLine("Day 3 - Mull It Over");
Console.WriteLine($"A: Product sum is {productSumA}."); // 161, 183788984
Console.WriteLine($"B: do to don't product sum is {productSumB}."); // 48, 62098619
