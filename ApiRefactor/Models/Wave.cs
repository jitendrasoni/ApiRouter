using Microsoft.Data.Sqlite;

namespace ApiRefactor.Models;

public class Waves
{
    public List<Wave> Items { get; internal set; }

    public Waves()
    {
        Items = new List<Wave>();
        SqliteConnection connection = SqlConnection.GetSqlConnection();
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "select id from waves";

        SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            var id = Guid.Parse(reader["id"].ToString());
            Items.Add(new Wave(id));
        }
    }
}

public class Wave
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTime WaveDate { get; set; }

    public Wave()
    {
        Id = Guid.NewGuid();
        WaveDate = DateTime.Now;
    }

    public Wave(Guid id)
    {
        SqliteConnection connection = SqlConnection.GetSqlConnection();
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "select * from waves where id = @id";
        command.Parameters.AddWithValue("@id", id);

        SqliteDataReader reader = command.ExecuteReader();
        if (reader.Read())
        {
            Id = Guid.Parse(reader["id"].ToString());
            Name = reader["name"].ToString();
            WaveDate = DateTime.Parse(reader["wavedate"].ToString());
        }

        connection.Close();
    }

    public void Save()
    {
        SqliteConnection connection = SqlConnection.GetSqlConnection();
        connection.Open();

        var command = connection.CreateCommand();
        var saveCommand = connection.CreateCommand();

        command.CommandText = "select * from waves where id = @id";
        command.Parameters.AddWithValue("@id", Id);

        SqliteDataReader reader = command.ExecuteReader();
        if (!reader.Read())
        {
            saveCommand.CommandText =
            "insert into waves (id, name, wavedate) values (@id, @name, @date)";
            saveCommand.Parameters.AddWithValue("@id", Id);
            saveCommand.Parameters.AddWithValue("@name", Name);
            saveCommand.Parameters.AddWithValue("@date", WaveDate);
        }
        else
        {
            saveCommand.CommandText =
            "update waves set name=@name, wavedate=@date where id=@id";
            saveCommand.Parameters.AddWithValue("@id", Id);
            saveCommand.Parameters.AddWithValue("@name", Name);
            saveCommand.Parameters.AddWithValue("@date", WaveDate);
        }

        saveCommand.ExecuteNonQuery();

        connection.Close();
    }
}
