using System.Collections.Generic;
using System.Linq;
using VideoApi.Common.Helpers;

namespace VideoApi.UnitTests.Helpers;

public class ConferenceHelperTests
{
    [Test]
    public void Should_generate_1000_unique_numbers()
    {
        var uniqueNumbers = new List<int>();
        for (var i = 0; i < 1000; i++)
        {
            var id = ConferenceHelper.GenerateGlobalRareNumber();
            id.Should().NotBeNullOrWhiteSpace();
            id.Length.Should().Be(8);
            int.TryParse(id, out var number).Should().BeTrue();
            uniqueNumbers.Add(number);
        }
        // Confirm all numbers unique
        uniqueNumbers.Distinct().Count().Should().Be(1000);
    }
}
