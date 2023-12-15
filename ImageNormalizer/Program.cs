namespace ImageNormalizer;

public class Program
{
	public static void Main(string[] args)
	{
		IApplicationRunner applicationRunner = new ApplicationRunner(args);
		applicationRunner.Run();
	}
}
