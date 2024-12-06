bool useExampleInput = true;

string inputFilename = useExampleInput
	? "exampleInput.txt"
	: "input.txt";

IList<(int Left, int Right)> leftAndRight = File
	.ReadAllLines(inputFilename)
	.Select(line => line.Split(' ', StringSplitOptions.RemoveEmptyEntries))
	.Select(numberStrings => (int.Parse(numberStrings[0]), int.Parse(numberStrings[1])))
	.ToList();
IList<int> sortedLeft = leftAndRight.Select(pair => pair.Left).Order().ToList();
IList<int> sortedRight = leftAndRight.Select(pair => pair.Right).Order().ToList();
int sumOfDistances = sortedLeft
	.Zip(sortedRight, (left, right) => Math.Abs(left - right))
	.Sum();
int sumOfSimilarities = sortedLeft
	.Sum(left => left * sortedRight.Count(right => right == left));

Console.WriteLine("Day 1");
Console.WriteLine($"A: Sum of distances is {sumOfDistances}."); // 11, 1882714
Console.WriteLine($"B: Sum of similarities is {sumOfSimilarities}."); // 31, 19437052
