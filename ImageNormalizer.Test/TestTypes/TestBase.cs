using System.IO;

namespace ImageNormalizer.Test.TestTypes;

public abstract class TestBase
{
	protected const string TestDataPath = "TestData";

	protected static string GetTestDirectoryPath(string testDirectoryName)
		=> Path.Combine(TestDataPath, testDirectoryName);

	protected static string GetTestDirectoryPath(
		string testDirectoryParentName, string testDirectoryChildName)
			=> Path.Combine(
				TestDataPath, testDirectoryParentName, testDirectoryChildName);

	protected static string GetTestFilePath(string testFileName)
		=> Path.Combine(TestDataPath, testFileName);

	protected static bool ExistsOutputFile(string outputFilePath)
		=> File.Exists(outputFilePath);

	protected static void DeleteOutputFile(string outputFilePath)
	{
		if (File.Exists(outputFilePath))
		{
			File.Delete(outputFilePath);
		}
	}

	protected static void DeleteOutputDirectory(string outputDirectoryPath)
	{
		if (Directory.Exists(outputDirectoryPath))
		{
			Directory.Delete(outputDirectoryPath, true);
		}
	}
}
