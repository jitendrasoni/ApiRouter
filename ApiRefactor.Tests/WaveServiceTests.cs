using ApiRefactor.Models;
using ApiRefactor.Repositories;
using ApiRefactor.Services;
using FluentAssertions;
using Moq;

namespace ApiRefactor.Tests;

public class WaveServiceTests
{
    private readonly Mock<IWaveRepository> _repoMock;
    private readonly WaveService _service;

    public WaveServiceTests()
    {
        _repoMock = new Mock<IWaveRepository>();
        _service = new WaveService(_repoMock.Object);
    }

    [Fact]
    public async Task GetWave_ReturnsWave_WhenExists()
    {
        var id = Guid.NewGuid();

        _repoMock.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync(new Wave { Id = id, Name = "Morning Wave" });

        var result = await _service.GetWaveAsync(id);

        result.Should().NotBeNull();
        result!.Name.Should().Be("Morning Wave");
    }

    [Fact]
    public async Task Upsert_ThrowsException_WhenNameMissing()
    {
        var wave = new Wave { Id = Guid.NewGuid(), Name = "" };

        Func<Task> act = async () => await _service.UpsertWaveAsync(wave);

        await act.Should().ThrowAsync<ArgumentException>();
    }
}
