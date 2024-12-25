bool useExampleInput = false;

string inputFilename = useExampleInput
	? "exampleInput.txt"
	: "input.txt";

List<string[]> locksAndKeys = File
	.ReadAllText(inputFilename)
	.Split("\r\n\r\n", StringSplitOptions.RemoveEmptyEntries)
	.Select(item => item.Split("\r\n"))
	.ToList();
List<List<int>> locks = locksAndKeys
	.Where(item => item[0][0] == '#')
	.Select(item => Enumerable
		.Range(0, item[0].Length)
		.Select(columnIndex => CountVerticalHeight(item, columnIndex, '#') - 1)
		.ToList())
	.ToList();
List<List<int>> keys = locksAndKeys
	.Where(item => item[0][0] == '.')
	.Select(item => Enumerable
		.Range(0, item[0].Length)
		.Select(columnIndex => 6 - CountVerticalHeight(item, columnIndex, '.'))
		.ToList())
	.ToList();
int resultPartA = locks.Sum(aLock =>
	keys.Count(aKey => aLock.Zip(aKey).All(item => item.First + item.Second <= 5)));

Console.WriteLine("Day 25 - Code Chronicle");
Console.WriteLine($"A: The number of unique lock/key pairs that fit is {resultPartA}."); // 3, 2854

int CountVerticalHeight(string[] matrix, int column, char charToCount) => Enumerable
	.Range(0, matrix.Length)
	.First(rowIndex => matrix[rowIndex][column] != charToCount);
