using System;
using ImageNormalizer.Logger;

namespace ImageNormalizer.Test.TestTypes.Stubs;

public class NullLogger : ILogger
{
    public void Info(string message)
    {
    }
    
    public void Error(string message)
    {
    }

    public void Error(Exception ex)
    {
    }
}
