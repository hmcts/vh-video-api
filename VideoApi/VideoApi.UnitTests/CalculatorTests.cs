using NUnit.Framework;
using VideoApi.Domain;

namespace VideoApi.UnitTests;

public class CalculatorTests
{
    [Test]
    public void should_add_two_numbers()
    {
        var calculator = new Calculator();
        Assert.AreEqual(2, calculator.Add(1, 1));
    }

    [Test]
    public void should_subtract_two_numbers()
    {
        var calculator = new Calculator();
        Assert.AreEqual(0, calculator.Subtract(1, 1));
    }

    [Test]
    public void should_multiply_two_numbers()
    {
        var calculator = new Calculator();
        Assert.AreEqual(1, calculator.Multiply(1, 1));
    }
}
