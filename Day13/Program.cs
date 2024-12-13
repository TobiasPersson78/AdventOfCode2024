using System.Text.RegularExpressions;

bool useExampleInput = false;

string inputFilename = useExampleInput
	? "exampleInput.txt"
	: "input.txt";

List<Machine> clawMachines = File
	.ReadAllText(inputFilename)
	.Split("\r\n\r\n")
	.Select(machine => Regex
		.Matches(machine, @"\d+")
		.Select(match => long.Parse(match.Groups[0].Value))
		.ToList())
	.Select(numbers => new Machine(numbers[0], numbers[1], numbers[2], numbers[3], numbers[4], numbers[5]))
	.ToList();
long resultPartA = clawMachines.Sum(SolveUsingCramersRule);
long resultPartB = clawMachines.Sum(item =>
	SolveUsingCramersRule(
		item with { TargetX = item.TargetX + 10000000000000L, TargetY = item.TargetY + 10000000000000L }));

Console.WriteLine("Day 13");
Console.WriteLine($"A: The fewest required tokens is {resultPartA}."); // 480, 27105
Console.WriteLine($"B: The fewest required tokens is {resultPartB}."); // 875318608908, 101726882250942

long SolveUsingCramersRule(Machine machine)
{
	long determinant = machine.ADeltaX * machine.BDeltaY - machine.ADeltaY * machine.BDeltaX;
	long numberOfPressesOnA = (machine.TargetX * machine.BDeltaY - machine.TargetY * machine.BDeltaX) / determinant;

	if (numberOfPressesOnA * determinant != (machine.TargetX * machine.BDeltaY - machine.TargetY * machine.BDeltaX))
		return 0; // No solution

	long numberOfPressesOnB = (machine.TargetY - machine.ADeltaY * numberOfPressesOnA) / machine.BDeltaY;

	return 3 * numberOfPressesOnA + numberOfPressesOnB;
}

record Machine(long ADeltaX, long ADeltaY, long BDeltaX, long BDeltaY, long TargetX, long TargetY);
