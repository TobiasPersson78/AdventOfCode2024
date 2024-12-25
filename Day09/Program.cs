bool useExampleInput = false;

string inputFilename = useExampleInput
	? "exampleInput.txt"
	: "input.txt";

string input = File.ReadAllText(inputFilename).Trim() + "0";
List<(int Id, int Length, int Empty, bool Moved)> fileIdLengthAndEmptyBlocks = Enumerable
	.Range(0, input.Length / 2)
	.Select(index => (Id: index, Length: input[2 * index] - '0', Empty: input[2 * index + 1] - '0', Moved: false))
	.ToList();
Func<IEnumerable<(int Id, int Length, int Empty, bool Moved)>, IEnumerable<int?>> singleBlocks = blocks => blocks
	.SelectMany(item =>
		Enumerable.Repeat<int?>(item.Id, item.Length)
			.Concat(Enumerable.Repeat<int?>(null, item.Empty)));
Func<IEnumerable<int?>, long> calculateChecksum = idUsingBlock => idUsingBlock
	.Select((item, index) => item is null ? 0 : item.Value * (long)index)
	.Sum();

List<int?> fileIdsInBlocks = singleBlocks(fileIdLengthAndEmptyBlocks).ToList();
for (int i = 0; i < fileIdsInBlocks.Count; ++i)
{
	if (fileIdsInBlocks[i] is null)
	{
		fileIdsInBlocks[i] = fileIdsInBlocks[^1];
		fileIdsInBlocks.RemoveAt(fileIdsInBlocks.Count - 1);
		while (fileIdsInBlocks[^1] is null)
			fileIdsInBlocks.RemoveAt(fileIdsInBlocks.Count - 1);
	}
}
long resultPartA = calculateChecksum(fileIdsInBlocks);

for (int endIndex = fileIdLengthAndEmptyBlocks.Count - 1; endIndex > 0; --endIndex)
{
	if (fileIdLengthAndEmptyBlocks[endIndex].Moved)
		continue;

	int startIndex = fileIdLengthAndEmptyBlocks.FindIndex(item => item.Empty >= fileIdLengthAndEmptyBlocks[endIndex].Length);

	if (startIndex >= 0 && startIndex < endIndex)
	{
		(int Id, int Length, int Empty, bool Moved) moved = fileIdLengthAndEmptyBlocks[endIndex] with
		{
			Empty = fileIdLengthAndEmptyBlocks[startIndex].Empty - fileIdLengthAndEmptyBlocks[endIndex].Length,
			Moved = true
		};
		fileIdLengthAndEmptyBlocks[endIndex - 1] = fileIdLengthAndEmptyBlocks[endIndex - 1] with
		{
			Empty =
				fileIdLengthAndEmptyBlocks[endIndex - 1].Empty +
				fileIdLengthAndEmptyBlocks[endIndex].Length +
				fileIdLengthAndEmptyBlocks[endIndex].Empty
		};
		fileIdLengthAndEmptyBlocks.RemoveAt(endIndex);
		fileIdLengthAndEmptyBlocks[startIndex] = fileIdLengthAndEmptyBlocks[startIndex] with { Empty = 0 };
		fileIdLengthAndEmptyBlocks.Insert(startIndex + 1, moved);
		++endIndex;
	}
}
long resultPartB = calculateChecksum(singleBlocks(fileIdLengthAndEmptyBlocks));

Console.WriteLine("Day 9 - Disk Fragmenter");
Console.WriteLine($"A: Checksum is {resultPartA}"); // 1928, 6216544403458
Console.WriteLine($"B: Checksum is {resultPartB}"); // 2858, 6237075041489
