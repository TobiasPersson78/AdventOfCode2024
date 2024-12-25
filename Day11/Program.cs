using System.Collections.Concurrent;

bool useExampleInput = false;

string inputFilename = useExampleInput
	? "exampleInput.txt"
	: "input.txt";

List<string> input = File
	.ReadAllLines(inputFilename)[0]
	.Split(' ')
	.ToList();
ConcurrentDictionary<(string Value, int RemainingSteps), long> lookupForValueAndRemainingSteps = new();

long Step(string value, int remainingSteps) =>
	lookupForValueAndRemainingSteps.GetOrAdd((value, remainingSteps), parameter =>
		parameter.RemainingSteps == 0 ? 1 :
		parameter.Value == "0" ? Step("1", parameter.RemainingSteps - 1) :
		parameter.Value.Length % 2 == 0
			? Step(parameter.Value[..(parameter.Value.Length / 2)], remainingSteps - 1) +
				Step(long.Parse(parameter.Value[(parameter.Value.Length / 2)..]).ToString(), remainingSteps - 1) :
		Step((long.Parse(parameter.Value) * 2024).ToString(), remainingSteps - 1));

long resultPartA = input.Sum(item => Step(item, 25));
long resultPartB = input.AsParallel().Sum(item => Step(item, 75));

Console.WriteLine("Day 11 - Plutonian Pebbles");
Console.WriteLine($"A: The number of stones after 25 blinks is {resultPartA}."); // 55312, 194557
Console.WriteLine($"B: The number of stones after 75 blinks is {resultPartB}."); // 65601038650482, 231532558973909
