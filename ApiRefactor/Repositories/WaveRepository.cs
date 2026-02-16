using ApiRefactor.Models;
using Microsoft.Data.Sqlite;

namespace ApiRefactor.Repositories;

public class WaveRepository : IWaveRepository
{
    public async Task<List<Wave>> GetAllAsync()
    {
        var waves = new List<Wave>();

        using var connection = SqlConnection.GetSqlConnection();
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = "select * from waves";

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            waves.Add(new Wave
            {
                Id = Guid.Parse(reader["id"].ToString()!),
                Name = reader["name"].ToString()!,
                WaveDate = DateTime.Parse(reader["wavedate"].ToString()!)
            });
        }

        return waves;
    }

    public async Task<Wave?> GetByIdAsync(Guid id)
    {
        using var connection = SqlConnection.GetSqlConnection();
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = "select * from waves where id=@id";
        command.Parameters.AddWithValue("@id", id);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Wave
            {
                Id = Guid.Parse(reader["id"].ToString()!),
                Name = reader["name"].ToString()!,
                WaveDate = DateTime.Parse(reader["wavedate"].ToString()!)
            };
        }

        return null;
    }

    public async Task UpsertAsync(Wave wave)
    {
        using var connection = SqlConnection.GetSqlConnection();
        await connection.OpenAsync();

        var existsCmd = connection.CreateCommand();
        existsCmd.CommandText = "select count(*) from waves where id=@id";
        existsCmd.Parameters.AddWithValue("@id", wave.Id);

        var exists = Convert.ToInt32(await existsCmd.ExecuteScalarAsync()) > 0;

        var cmd = connection.CreateCommand();

        if (!exists)
        {
            cmd.CommandText =
                "insert into waves (id,name,wavedate) values(@id,@name,@date)";
        }
        else
        {
            cmd.CommandText =
                "update waves set name=@name,wavedate=@date where id=@id";
        }

        cmd.Parameters.AddWithValue("@id", wave.Id);
        cmd.Parameters.AddWithValue("@name", wave.Name);
        cmd.Parameters.AddWithValue("@date", wave.WaveDate);

        await cmd.ExecuteNonQueryAsync();
    }
}
