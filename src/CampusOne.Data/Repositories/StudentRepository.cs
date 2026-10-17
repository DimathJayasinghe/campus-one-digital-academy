using CampusOne.Core.Interfaces;
using CampusOne.Core.Models;
using Microsoft.Data.SqlClient;

namespace CampusOne.Data.Repositories;

/// <summary>
/// ADO.NET implementation of <see cref="IStudentRepository"/>.
/// All SQL is written inline (no ORM) to keep the project simple.
/// </summary>
public class StudentRepository : IStudentRepository
{
    private readonly DbConnectionFactory _factory;

    public StudentRepository(DbConnectionFactory factory)
    {
        _factory = factory;
    }

    /// <inheritdoc/>
    public IEnumerable<Student> GetAll()
    {
        var students = new List<Student>();
        const string sql = "SELECT * FROM Registration ORDER BY regNo";

        using var conn = _factory.Create();
        using var cmd = new SqlCommand(sql, conn);
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
            students.Add(MapRow(reader));

        return students;
    }

    /// <inheritdoc/>
    public Student? GetByRegNo(int regNo)
    {
        const string sql = "SELECT * FROM Registration WHERE regNo = @regNo";

        using var conn = _factory.Create();
        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@regNo", regNo);

        using var reader = cmd.ExecuteReader();
        return reader.Read() ? MapRow(reader) : null;
    }

    /// <inheritdoc/>
    public void Add(Student s)
    {
        const string sql = @"
            INSERT INTO Registration
                (regNo, firstName, lastName, dateOfBirth, gender, address,
                 email, mobilePhone, homePhone, parentName, nic, contactNo)
            VALUES
                (@regNo, @firstName, @lastName, @dateOfBirth, @gender, @address,
                 @email, @mobilePhone, @homePhone, @parentName, @nic, @contactNo)";

        using var conn = _factory.Create();
        using var cmd = new SqlCommand(sql, conn);
        BindParameters(cmd, s);
        cmd.ExecuteNonQuery();
    }

    /// <inheritdoc/>
    public void Update(Student s)
    {
        const string sql = @"
            UPDATE Registration SET
                firstName   = @firstName,
                lastName    = @lastName,
                dateOfBirth = @dateOfBirth,
                gender      = @gender,
                address     = @address,
                email       = @email,
                mobilePhone = @mobilePhone,
                homePhone   = @homePhone,
                parentName  = @parentName,
                nic         = @nic,
                contactNo   = @contactNo
            WHERE regNo = @regNo";

        using var conn = _factory.Create();
        using var cmd = new SqlCommand(sql, conn);
        BindParameters(cmd, s);
        cmd.ExecuteNonQuery();
    }

    /// <inheritdoc/>
    public void Delete(int regNo)
    {
        const string sql = "DELETE FROM Registration WHERE regNo = @regNo";

        using var conn = _factory.Create();
        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@regNo", regNo);
        cmd.ExecuteNonQuery();
    }

    /// <inheritdoc/>
    public IEnumerable<int> GetAllRegNos()
    {
        var list = new List<int>();
        const string sql = "SELECT regNo FROM Registration ORDER BY regNo";

        using var conn = _factory.Create();
        using var cmd = new SqlCommand(sql, conn);
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
            list.Add(reader.GetInt32(0));

        return list;
    }

    // ── helpers ──────────────────────────────────────────────────────────────

    private static Student MapRow(SqlDataReader r) => new()
    {
        RegNo       = r.GetInt32(r.GetOrdinal("regNo")),
        FirstName   = r.GetString(r.GetOrdinal("firstName")),
        LastName    = r.GetString(r.GetOrdinal("lastName")),
        DateOfBirth = r.GetDateTime(r.GetOrdinal("dateOfBirth")),
        Gender      = r.GetString(r.GetOrdinal("gender")),
        Address     = r.GetString(r.GetOrdinal("address")),
        Email       = r.GetString(r.GetOrdinal("email")),
        MobilePhone = r.GetInt64(r.GetOrdinal("mobilePhone")),
        HomePhone   = r.IsDBNull(r.GetOrdinal("homePhone")) ? 0 : r.GetInt64(r.GetOrdinal("homePhone")),
        ParentName  = r.IsDBNull(r.GetOrdinal("parentName")) ? string.Empty : r.GetString(r.GetOrdinal("parentName")),
        Nic         = r.IsDBNull(r.GetOrdinal("nic")) ? string.Empty : r.GetString(r.GetOrdinal("nic")),
        ContactNo   = r.IsDBNull(r.GetOrdinal("contactNo")) ? 0 : r.GetInt64(r.GetOrdinal("contactNo")),
    };

    private static void BindParameters(SqlCommand cmd, Student s)
    {
        cmd.Parameters.AddWithValue("@regNo",       s.RegNo);
        cmd.Parameters.AddWithValue("@firstName",   s.FirstName);
        cmd.Parameters.AddWithValue("@lastName",    s.LastName);
        cmd.Parameters.AddWithValue("@dateOfBirth", s.DateOfBirth);
        cmd.Parameters.AddWithValue("@gender",      s.Gender);
        cmd.Parameters.AddWithValue("@address",     s.Address);
        cmd.Parameters.AddWithValue("@email",       s.Email);
        cmd.Parameters.AddWithValue("@mobilePhone", s.MobilePhone);
        cmd.Parameters.AddWithValue("@homePhone",   s.HomePhone);
        cmd.Parameters.AddWithValue("@parentName",  s.ParentName);
        cmd.Parameters.AddWithValue("@nic",         s.Nic);
        cmd.Parameters.AddWithValue("@contactNo",   s.ContactNo);
    }
}
