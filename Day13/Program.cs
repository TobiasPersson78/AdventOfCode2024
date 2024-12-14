using System.Text.RegularExpressions;

bool useExampleInput = false;

string inputFilename = useExampleInput
	? "exampleInput.txt"
	: "input.txt";

List<Machine> clawMachines = Regex
	.Matches(File.ReadAllText(inputFilename), @"\d+")
	.Select(match => long.Parse(match.Groups[0].Value))
	.Chunk(6)
	.Select(item => new Machine(item[0], item[1], item[2], item[3], item[4], item[5]))
	.ToList();
long resultPartA = clawMachines.Sum(SolveLinearEquation);
long resultPartB = clawMachines.Sum(item =>
	SolveLinearEquation(
		item with { TargetX = item.TargetX + 10000000000000L, TargetY = item.TargetY + 10000000000000L }));

Console.WriteLine("Day 13");
Console.WriteLine($"A: The fewest required tokens is {resultPartA}."); // 480, 27105
Console.WriteLine($"B: The fewest required tokens is {resultPartB}."); // 875318608908, 101726882250942

long SolveLinearEquation(Machine machine)
{
	long determinant = machine.ADeltaX * machine.BDeltaY - machine.ADeltaY * machine.BDeltaX;
	long numberOfPressesOnA = (machine.TargetX * machine.BDeltaY - machine.TargetY * machine.BDeltaX) / determinant;

	if (numberOfPressesOnA * determinant != machine.TargetX * machine.BDeltaY - machine.TargetY * machine.BDeltaX)
		return 0; // No integer solution. Could also check for >= 0. Could also check that numberOfPressesOnB also is integer >= 0.

	long numberOfPressesOnB = (machine.TargetY - machine.ADeltaY * numberOfPressesOnA) / machine.BDeltaY;

	return 3 * numberOfPressesOnA + numberOfPressesOnB;
}

record Machine(long ADeltaX, long ADeltaY, long BDeltaX, long BDeltaY, long TargetX, long TargetY);
