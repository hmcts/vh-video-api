using Testing.Common.Helper.Builders.Domain;

namespace VideoApi.UnitTests.Domain.Conference;

public class GetIngestUrlFilenamePrefixTests
{
    [Test]
    public void Should_return_ingest_url_filename_prefix()
    {
        // Arrange
        var conference = new ConferenceBuilder().Build();
        conference.IngestUrl =
            "rtmps://platform.net:443/vh-recording-app/ABA4-1040Test-c388a080-53d1-4899-98cc-569ba6213244";

        // Act
        var prefix = conference.IngestUrlFilenamePrefix;

        // Assert
        prefix.Should().Be("ABA4-1040Test-c388a080-53d1-4899-98cc-569ba6213244");
    }

    [Test]
    public void Should_return_null_when_ingest_url_is_null()
    {
        // Arrange
        var conference = new ConferenceBuilder().Build();
        conference.IngestUrl = null;
        
        // Act
        var prefix = conference.IngestUrlFilenamePrefix;
        
        // Assert
        prefix.Should().BeNull();
    }

    [Test]
    public void Should_return_empty_string_when_ingest_url_is_empty_string()
    {
        // Arrange
        var conference = new ConferenceBuilder().Build();
        conference.IngestUrl = string.Empty;
        
        // Act
        var prefix = conference.IngestUrlFilenamePrefix;
        
        // Assert
        prefix.Should().Be(string.Empty);
    }
}
