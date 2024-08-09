using Microsoft.Extensions.Logging;
using Moq;

namespace InterviewProject.Tests.Base
{
    public class BaseTests
    {
        public void IsExpectedLog<TLoggerMock>(Mock<ILogger<TLoggerMock>> loggerMock,
            string expectedMessage,
            Func<Times> expectedTimes = null,
            Enum expectedLogLevel = null)
            where TLoggerMock : class
        {
            expectedLogLevel = expectedLogLevel ?? LogLevel.Information;
            expectedTimes = expectedTimes ?? Times.Once;

            loggerMock
                .Verify(
                    x => x.Log(
                        It.Is<LogLevel>(l => l == (LogLevel)expectedLogLevel),
                        It.IsAny<EventId>(),
                        It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(expectedMessage)),
                        It.IsAny<Exception>(),
                        It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), expectedTimes
                );
        }
    }
}