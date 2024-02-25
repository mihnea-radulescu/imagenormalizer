using System.IO;

namespace ImageNormalizer.Test.TestTypes;

public abstract class TestBase
{
    protected const string TestDataPath = "TestData";

	protected string GetTestDirectoryPath(string testDirectoryName)
        => Path.Combine(TestDataPath, testDirectoryName);

	protected string GetTestDirectoryPath(
        string testDirectoryParentName, string testDirectoryChildName)
		    => Path.Combine(TestDataPath, testDirectoryParentName, testDirectoryChildName);

	protected string GetTestFilePath(string testFileName)
        => Path.Combine(TestDataPath, testFileName);

	protected bool ExistsOutputFile(string outputFilePath) => File.Exists(outputFilePath);

    protected void DeleteOutputFile(string outputFilePath)
    {
        if (File.Exists(outputFilePath))
        {
            File.Delete(outputFilePath);
        }
    }

    protected void DeleteOutputDirectory(string outputDirectoryPath)
    {
        if (Directory.Exists(outputDirectoryPath))
        {
            Directory.Delete(outputDirectoryPath, true);
        }
    }
}
