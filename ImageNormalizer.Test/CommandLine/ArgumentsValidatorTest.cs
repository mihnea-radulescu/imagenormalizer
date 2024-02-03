using Xunit;
using ImageNormalizer.CommandLine;
using ImageNormalizer.Test.TestTypes;

namespace ImageNormalizer.Test.CommandLine;

public class ArgumentsValidatorTest : TestBase
{
    public ArgumentsValidatorTest()
    {
        _argumentsValidator = new ArgumentsValidator();
    }

	[Fact]
	public void AreValidArguments_ValidInputDistinctNameRoot_ReturnsTrueWithNullErrorMessage()
	{
		// Arrange
		var inputDirectory = GetTestDirectoryPath("Subfolder1");
		var outputDirectory = GetTestDirectoryPath("Subfolder2");
		const int outputMaximumImageSize = 3840;
		const int outputImageQuality = 80;

		var arguments = new Arguments(
			inputDirectory, outputDirectory, outputMaximumImageSize, outputImageQuality);

		// Act
		var areValidArguments = _argumentsValidator.AreValidArguments(
			arguments, out string? errorMessage);

		// Assert
		Assert.True(areValidArguments);
		Assert.Null(errorMessage);
	}

	[Fact]
	public void AreValidArguments_ValidInputCommonNameRoot_ReturnsTrueWithNullErrorMessage()
	{
		// Arrange
		var inputDirectory = GetTestDirectoryPath("Subfolder1");
		var outputDirectory = GetTestDirectoryPath("Subfolder1_Output");
		const int outputMaximumImageSize = 3840;
		const int outputImageQuality = 80;

		var arguments = new Arguments(
			inputDirectory, outputDirectory, outputMaximumImageSize, outputImageQuality);

		// Act
		var areValidArguments = _argumentsValidator.AreValidArguments(
			arguments, out string? errorMessage);

		// Assert
		Assert.True(areValidArguments);
		Assert.Null(errorMessage);
	}

	[Fact]
	public void AreValidArguments_InvalidInputDirectoryPath_ReturnsFalseWithExpectedErrorMessage()
	{
		// Arrange
		var inputDirectory = ";";
		var outputDirectory = GetTestDirectoryPath("Subfolder1_Output");
		const int outputMaximumImageSize = 3840;
		const int outputImageQuality = 80;

		var arguments = new Arguments(
			inputDirectory, outputDirectory, outputMaximumImageSize, outputImageQuality);

		// Act
		var areValidArguments = _argumentsValidator.AreValidArguments(
			arguments, out string? errorMessage);

		// Assert
		Assert.False(areValidArguments);
		Assert.NotNull(errorMessage);
		Assert.True(
			errorMessage.Contains("The directory path") &&
			errorMessage.Contains("is invalid."));
	}

	[Fact]
	public void AreValidArguments_InvalidOutputDirectoryPath_ReturnsFalseWithExpectedErrorMessage()
	{
		// Arrange
		var inputDirectory = GetTestDirectoryPath("Subfolder1");
		var outputDirectory = ";";
		const int outputMaximumImageSize = 3840;
		const int outputImageQuality = 80;

		var arguments = new Arguments(
			inputDirectory, outputDirectory, outputMaximumImageSize, outputImageQuality);

		// Act
		var areValidArguments = _argumentsValidator.AreValidArguments(
			arguments, out string? errorMessage);

		// Assert
		Assert.False(areValidArguments);
		Assert.NotNull(errorMessage);
		Assert.True(
			errorMessage.Contains("The directory path") &&
			errorMessage.Contains("is invalid."));
	}

	[Fact]
	public void AreValidArguments_InputDirectoryDoesNotExist_ReturnsFalseWithExpectedErrorMessage()
	{
		// Arrange
		var inputDirectory = "nonexistent_folder";
		var outputDirectory = GetTestDirectoryPath("Subfolder1");
		const int outputMaximumImageSize = 3840;
		const int outputImageQuality = 80;

		var arguments = new Arguments(
			inputDirectory, outputDirectory, outputMaximumImageSize, outputImageQuality);

		// Act
		var areValidArguments = _argumentsValidator.AreValidArguments(
			arguments, out string? errorMessage);

		// Assert
		Assert.False(areValidArguments);
		Assert.NotNull(errorMessage);
		Assert.True(
			errorMessage.Contains("The input directory") &&
			errorMessage.Contains("does not exist."));
	}

	[Fact]
	public void AreValidArguments_IdenticalInputAndOutputDirectories_ReturnsFalseWithExpectedErrorMessage()
	{
		// Arrange
		var inputDirectory = GetTestDirectoryPath("Subfolder1");
		var outputDirectory = GetTestDirectoryPath("Subfolder1");
		const int outputMaximumImageSize = 3840;
		const int outputImageQuality = 80;

		var arguments = new Arguments(
			inputDirectory, outputDirectory, outputMaximumImageSize, outputImageQuality);

		// Act
		var areValidArguments = _argumentsValidator.AreValidArguments(
			arguments, out string? errorMessage);

		// Assert
		Assert.False(areValidArguments);
		Assert.NotNull(errorMessage);
		Assert.True(
			errorMessage.Contains("The input directory") &&
			errorMessage.Contains("and output directory") &&
			errorMessage.Contains("are identical."));
	}

	[Fact]
	public void AreValidArguments_InputDirectoryIsParentOfOutputDirectory_ReturnsFalseWithExpectedErrorMessage()
	{
		// Arrange
		var inputDirectory = GetTestDirectoryPath("Subfolder2");
		var outputDirectory = GetTestDirectoryPath("Subfolder2", "Subfolder2_1");
		const int outputMaximumImageSize = 3840;
		const int outputImageQuality = 80;

		var arguments = new Arguments(
			inputDirectory, outputDirectory, outputMaximumImageSize, outputImageQuality);

		// Act
		var areValidArguments = _argumentsValidator.AreValidArguments(
			arguments, out string? errorMessage);

		// Assert
		Assert.False(areValidArguments);
		Assert.NotNull(errorMessage);
		Assert.True(
			errorMessage.Contains("The input directory") &&
			errorMessage.Contains("and output directory") &&
			errorMessage.Contains("are in a parent-child relationship."));
	}

	[Fact]
	public void AreValidArguments_OutputDirectoryIsParentOfInputDirectory_ReturnsFalseWithExpectedErrorMessage()
	{
		// Arrange
		var inputDirectory = GetTestDirectoryPath("Subfolder2", "Subfolder2_1");
		var outputDirectory = GetTestDirectoryPath("Subfolder2");
		const int outputMaximumImageSize = 3840;
		const int outputImageQuality = 80;

		var arguments = new Arguments(
			inputDirectory, outputDirectory, outputMaximumImageSize, outputImageQuality);

		// Act
		var areValidArguments = _argumentsValidator.AreValidArguments(
			arguments, out string? errorMessage);

		// Assert
		Assert.False(areValidArguments);
		Assert.NotNull(errorMessage);
		Assert.True(
			errorMessage.Contains("The input directory") &&
			errorMessage.Contains("and output directory") &&
			errorMessage.Contains("are in a parent-child relationship."));
	}

	[Fact]
	public void AreValidArguments_OutputMaximumImageSizeIsBelowThreshold_ReturnsFalseWithExpectedErrorMessage()
	{
		// Arrange
		var inputDirectory = GetTestDirectoryPath("Subfolder1");
		var outputDirectory = GetTestDirectoryPath("Subfolder2");
		const int outputMaximumImageSize = 9;
		const int outputImageQuality = 80;

		var arguments = new Arguments(
			inputDirectory, outputDirectory, outputMaximumImageSize, outputImageQuality);

		// Act
		var areValidArguments = _argumentsValidator.AreValidArguments(
			arguments, out string? errorMessage);

		// Assert
		Assert.False(areValidArguments);
		Assert.NotNull(errorMessage);
		Assert.Equal(
			"The output maximum image size is outside of the expected range.",
			errorMessage);
	}

	[Fact]
	public void AreValidArguments_OutputMaximumImageSizeIsAboveThreshold_ReturnsFalseWithExpectedErrorMessage()
	{
		// Arrange
		var inputDirectory = GetTestDirectoryPath("Subfolder1");
		var outputDirectory = GetTestDirectoryPath("Subfolder2");
		const int outputMaximumImageSize = 10001;
		const int outputImageQuality = 80;

		var arguments = new Arguments(
			inputDirectory, outputDirectory, outputMaximumImageSize, outputImageQuality);

		// Act
		var areValidArguments = _argumentsValidator.AreValidArguments(
			arguments, out string? errorMessage);

		// Assert
		Assert.False(areValidArguments);
		Assert.NotNull(errorMessage);
		Assert.Equal(
			"The output maximum image size is outside of the expected range.",
			errorMessage);
	}

	[Fact]
	public void AreValidArguments_OutputImageQualityIsBelowThreshold_ReturnsFalseWithExpectedErrorMessage()
	{
		// Arrange
		var inputDirectory = GetTestDirectoryPath("Subfolder1");
		var outputDirectory = GetTestDirectoryPath("Subfolder2");
		const int outputMaximumImageSize = 3840;
		const int outputImageQuality = 0;

		var arguments = new Arguments(
			inputDirectory, outputDirectory, outputMaximumImageSize, outputImageQuality);

		// Act
		var areValidArguments = _argumentsValidator.AreValidArguments(
			arguments, out string? errorMessage);

		// Assert
		Assert.False(areValidArguments);
		Assert.NotNull(errorMessage);
		Assert.Equal(
			"The output image quality is outside of the expected range.",
			errorMessage);
	}

	[Fact]
	public void AreValidArguments_OutputImageQualityIsAboveThreshold_ReturnsFalseWithExpectedErrorMessage()
	{
		// Arrange
		var inputDirectory = GetTestDirectoryPath("Subfolder1");
		var outputDirectory = GetTestDirectoryPath("Subfolder2");
		const int outputMaximumImageSize = 3840;
		const int outputImageQuality = 101;

		var arguments = new Arguments(
			inputDirectory, outputDirectory, outputMaximumImageSize, outputImageQuality);

		// Act
		var areValidArguments = _argumentsValidator.AreValidArguments(
			arguments, out string? errorMessage);

		// Assert
		Assert.False(areValidArguments);
		Assert.NotNull(errorMessage);
		Assert.Equal(
			"The output image quality is outside of the expected range.",
			errorMessage);
	}

	#region Private

	private readonly ArgumentsValidator _argumentsValidator;

    #endregion
}
