using System.Collections.Concurrent;

bool useExampleInput = false;

string inputFilename = useExampleInput
	? "exampleInput.txt"
	: "input.txt";

string[] inputLines = File.ReadAllLines(inputFilename);
List<string> patterns = inputLines[0].Split(", ").ToList();
string[] designs = inputLines[2..];
ConcurrentDictionary<(string Design, int Offset), long> numberOfMatchesForDesignAndOffset = new();

int resultPartA = designs.AsParallel().Count(item => NumberOfMatches(item, 0) > 0);
long resultPartB = designs.AsParallel().Sum(item => NumberOfMatches(item, 0));

Console.WriteLine("Day 19");
Console.WriteLine($"A: The number of possible designs is {resultPartA}."); // 6, 344
Console.WriteLine($"B: The number of ways to arrange all possible designs is {resultPartB}."); // 16, 996172272010026

long NumberOfMatches(string design, int offset) =>
	offset == design.Length
		? 1
		: numberOfMatchesForDesignAndOffset.GetOrAdd((design, offset), _ =>
			patterns.Sum(item =>
				offset + item.Length > design.Length || design[offset..(offset + item.Length)] != item
					? 0
					: NumberOfMatches(design, offset + item.Length)));
