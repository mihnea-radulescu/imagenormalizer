using System.IO;
using Xunit;
using ImageNormalizer.CommandLine;
using ImageNormalizer.Test.TestTypes;

namespace ImageNormalizer.Test.CommandLine;

public class CommandLineParserTest : TestBase
{
    public CommandLineParserTest()
    {
        _commandLineParser = new CommandLineParser();
    }

	[Fact]
	public void GetCommandLineArguments_ValidInputTwoArgumentsDistinctNameRoot_ReturnsExpectedInstance()
	{
		// Arrange
		var inputDirectory = GetTestDirectoryPath("Subfolder1");
		var outputDirectory = GetTestDirectoryPath("Subfolder2");

		var args = new[] { inputDirectory, outputDirectory };

		// Act
		var commandLineArguments = _commandLineParser.GetCommandLineArguments(args);

		// Assert
		Assert.False(commandLineArguments is null);
		Assert.Equal(Path.GetFullPath(inputDirectory), commandLineArguments.InputDirectory);
		Assert.Equal(Path.GetFullPath(outputDirectory), commandLineArguments.OutputDirectory);
		Assert.Equal(80, commandLineArguments.OutputImageQuality);
	}

	[Fact]
	public void GetCommandLineArguments_ValidInputTwoArgumentsCommonNameRoot_ReturnsExpectedInstance()
	{
		// Arrange
		var inputDirectory = GetTestDirectoryPath("Subfolder1");
		var outputDirectory = GetTestDirectoryPath("Subfolder1_Output");

		var args = new[] { inputDirectory, outputDirectory };

        // Act
        var commandLineArguments = _commandLineParser.GetCommandLineArguments(args);

        // Assert
        Assert.False(commandLineArguments is null);
        Assert.Equal(Path.GetFullPath(inputDirectory), commandLineArguments.InputDirectory);
		Assert.Equal(Path.GetFullPath(outputDirectory), commandLineArguments.OutputDirectory);
		Assert.Equal(80, commandLineArguments.OutputImageQuality);
	}

	[Fact]
	public void GetCommandLineArguments_ValidInputThreeArgumentsDistinctNameRoot_ReturnsExpectedInstance()
	{
		// Arrange
		var inputDirectory = GetTestDirectoryPath("Subfolder1");
		var outputDirectory = GetTestDirectoryPath("Subfolder2");
		var outputImageQuality = 80;

		var args = new[] { inputDirectory, outputDirectory, outputImageQuality.ToString() };

		// Act
		var commandLineArguments = _commandLineParser.GetCommandLineArguments(args);

		// Assert
		Assert.False(commandLineArguments is null);
		Assert.Equal(Path.GetFullPath(inputDirectory), commandLineArguments.InputDirectory);
		Assert.Equal(Path.GetFullPath(outputDirectory), commandLineArguments.OutputDirectory);
		Assert.Equal(outputImageQuality, commandLineArguments.OutputImageQuality);
	}

	[Fact]
	public void GetCommandLineArguments_ValidInputThreeArgumentsCommonNameRoot_ReturnsExpectedInstance()
	{
		// Arrange
		var inputDirectory = GetTestDirectoryPath("Subfolder1");
		var outputDirectory = GetTestDirectoryPath("Subfolder1_Output");
		var outputImageQuality = 80;

		var args = new[] { inputDirectory, outputDirectory, outputImageQuality.ToString() };

		// Act
		var commandLineArguments = _commandLineParser.GetCommandLineArguments(args);

		// Assert
		Assert.False(commandLineArguments is null);
		Assert.Equal(Path.GetFullPath(inputDirectory), commandLineArguments.InputDirectory);
		Assert.Equal(Path.GetFullPath(outputDirectory), commandLineArguments.OutputDirectory);
		Assert.Equal(outputImageQuality, commandLineArguments.OutputImageQuality);
	}

	[Fact]
	public void GetCommandLineArguments_InvalidArgumentCountNoArguments_ReturnsNull()
	{
		// Arrange
		var args = new string[] { };

		// Act
		var commandLineArguments = _commandLineParser.GetCommandLineArguments(args);

		// Assert
		Assert.True(commandLineArguments is null);
	}

	[Fact]
	public void GetCommandLineArguments_InvalidArgumentCountOneArgument_ReturnsNull()
	{
		// Arrange
		var args = new[] { "fake_value" };

		// Act
		var commandLineArguments = _commandLineParser.GetCommandLineArguments(args);

		// Assert
		Assert.True(commandLineArguments is null);
	}

	[Fact]
	public void GetCommandLineArguments_InvalidArgumentCountFourArguments_ReturnsNull()
	{
		// Arrange
		var args = new[] { "fake_value_1", "fake_value_2", "fake_value_3", "fake_value_4" };

		// Act
		var commandLineArguments = _commandLineParser.GetCommandLineArguments(args);

		// Assert
		Assert.True(commandLineArguments is null);
	}

	[Fact]
	public void GetCommandLineArguments_InvalidArguments_ReturnsNull()
	{
		// Arrange
		var args = new[] { "", " " };

		// Act
		var commandLineArguments = _commandLineParser.GetCommandLineArguments(args);

		// Assert
		Assert.True(commandLineArguments is null);
	}

	[Fact]
	public void GetCommandLineArguments_NonexistentInputDirectory_ReturnsNull()
	{
		// Arrange
		var inputDirectory = "nonexistent_folder";
		var outputDirectory = GetTestDirectoryPath("Subfolder1");

		var args = new[] { inputDirectory, outputDirectory };

		// Act
		var commandLineArguments = _commandLineParser.GetCommandLineArguments(args);

		// Assert
		Assert.True(commandLineArguments is null);
	}

	[Fact]
	public void GetCommandLineArguments_IdenticalInputAndOutputDirectories_ReturnsNull()
	{
		// Arrange
		var inputDirectory = GetTestDirectoryPath("Subfolder1");
		var outputDirectory = GetTestDirectoryPath("Subfolder1");

		var args = new[] { inputDirectory, outputDirectory };

		// Act
		var commandLineArguments = _commandLineParser.GetCommandLineArguments(args);

		// Assert
		Assert.True(commandLineArguments is null);
	}

	[Fact]
	public void GetCommandLineArguments_InputDirectoryIsParentOfOutputDirectory_ReturnsNull()
	{
		// Arrange
		var inputDirectory = GetTestDirectoryPath("Subfolder2");
		var outputDirectory = GetTestDirectoryPath("Subfolder2", "Subfolder2_1");

		var args = new[] { inputDirectory, outputDirectory };

		// Act
		var commandLineArguments = _commandLineParser.GetCommandLineArguments(args);

		// Assert
		Assert.True(commandLineArguments is null);
	}

	[Fact]
	public void GetCommandLineArguments_OutputDirectoryIsParentOfInputDirectory_ReturnsNull()
	{
		// Arrange
		var inputDirectory = GetTestDirectoryPath("Subfolder2", "Subfolder2_1");
		var outputDirectory = GetTestDirectoryPath("Subfolder2");

		var args = new[] { inputDirectory, outputDirectory };

		// Act
		var commandLineArguments = _commandLineParser.GetCommandLineArguments(args);

		// Assert
		Assert.True(commandLineArguments is null);
	}

	[Fact]
	public void GetCommandLineArguments_OutputImageQualityBelowThreshold_ReturnsNull()
	{
		// Arrange
		var inputDirectory = GetTestDirectoryPath("Subfolder1");
		var outputDirectory = GetTestDirectoryPath("Subfolder2");
		var outputImageQuality = 0;

		var args = new[] { inputDirectory, outputDirectory, outputImageQuality.ToString() };

		// Act
		var commandLineArguments = _commandLineParser.GetCommandLineArguments(args);

		// Assert
		Assert.True(commandLineArguments is null);
	}

	[Fact]
	public void GetCommandLineArguments_OutputImageQualityAboveThreshold_ReturnsNull()
	{
		// Arrange
		var inputDirectory = GetTestDirectoryPath("Subfolder1");
		var outputDirectory = GetTestDirectoryPath("Subfolder2");
		var outputImageQuality = 101;

		var args = new[] { inputDirectory, outputDirectory, outputImageQuality.ToString() };

		// Act
		var commandLineArguments = _commandLineParser.GetCommandLineArguments(args);

		// Assert
		Assert.True(commandLineArguments is null);
	}

	#region Private

	private readonly CommandLineParser _commandLineParser;

    #endregion
}
