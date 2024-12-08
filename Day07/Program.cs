using System.Collections.Immutable;

bool useExampleInput = false;

string inputFilename = useExampleInput
	? "exampleInput.txt"
	: "input.txt";

ImmutableList<(long Target, ImmutableList<long> Operands)> input = File
	.ReadAllLines(inputFilename)
	.Select(line => line.Split(": "))
	.Select(item => (long.Parse(item[0]), item[1].Split(' ').Select(long.Parse).ToImmutableList()))
	.ToImmutableList();

bool IsMatch(long target, long current, ImmutableList<long> operands, bool useConcatenationOperator) =>
	(operands.Count == 0 && target == current) ||
	(operands.Count > 0 &&
		(IsMatch(target, current + operands[0], operands.RemoveAt(0), useConcatenationOperator) ||
		IsMatch(target, current * operands[0], operands.RemoveAt(0), useConcatenationOperator) ||
		(useConcatenationOperator &&
			IsMatch(target, long.Parse(current.ToString() + operands[0]), operands.RemoveAt(0), useConcatenationOperator))));

long resultPartA = input.Sum(item => IsMatch(item.Target, 0, item.Operands, false) ? item.Target : 0);
long resultPartB = input.Sum(item => IsMatch(item.Target, 0, item.Operands, true) ? item.Target : 0);

Console.WriteLine("Day 7");
Console.WriteLine($"A: {resultPartA}."); // 3749, 5702958180383
Console.WriteLine($"B: {resultPartB}."); // 11387, 92612386119138
