using System.Data;
using Banyan.Data.Models;
using Banyan.Entities.Models.clSchema;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Banyan.Repository.Models;

public interface IClientRepository
{
    Task<IReadOnlyList<Client_GetResult>> GetAllAsync(
        byte[] token,
        string secretKey,
        int sessionId,
        CancellationToken cancellationToken = default);
}

public sealed class ClientRepository : IClientRepository
{
    private readonly BanyanFinContext _context;

    public ClientRepository(BanyanFinContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Client_GetResult>> GetAllAsync(
        byte[] token,
        string secretKey,
        int sessionId,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(token);
        ArgumentException.ThrowIfNullOrWhiteSpace(secretKey);
        if (token.Length is 0 or > 256)
        {
            throw new ArgumentOutOfRangeException(nameof(token), "The session token must contain between 1 and 256 bytes.");
        }

        if (secretKey.Length > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(secretKey), "The configured secret key exceeds the stored procedure limit.");
        }

        var clients = new List<Client_GetResult>();
        var connection = _context.Database.GetDbConnection();
        var openedHere = connection.State == ConnectionState.Closed;
        if (openedHere)
        {
            await _context.Database.OpenConnectionAsync(cancellationToken);
        }

        try
        {
            // Client_Get uses @ID only to infer @IsInstitute when it is null; the explicit type selects the full list.
            await ReadClientTypeAsync(isInstitute: false);
            await ReadClientTypeAsync(isInstitute: true);
        }
        finally
        {
            if (openedHere)
            {
                await _context.Database.CloseConnectionAsync();
            }
        }

        return clients;

        async Task ReadClientTypeAsync(bool isInstitute)
        {
            await using var command = new SqlCommand("[cl].[Client_Get]", (SqlConnection)connection)
            {
                CommandType = CommandType.StoredProcedure,
                Transaction = _context.Database.CurrentTransaction?.GetDbTransaction() as SqlTransaction
            };

            command.Parameters.Add("@Token", SqlDbType.VarBinary, 256).Value = token;
            command.Parameters.Add("@SecretKey", SqlDbType.VarChar, 100).Value = secretKey;
            command.Parameters.Add("@ID", SqlDbType.Int).Value = sessionId;
            command.Parameters.Add("@IsInstitute", SqlDbType.Bit).Value = isInstitute;

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            var ordinals = Enumerable.Range(0, reader.FieldCount)
                .ToDictionary(reader.GetName, ordinal => ordinal, StringComparer.OrdinalIgnoreCase);

            while (await reader.ReadAsync(cancellationToken))
            {
                clients.Add(MapClient(reader, ordinals));
            }
        }
    }

    private static Client_GetResult MapClient(
        SqlDataReader reader,
        IReadOnlyDictionary<string, int> ordinals)
    {
        return new Client_GetResult
        {
            ID = ReadRequired<int>(reader, ordinals, nameof(Client_GetResult.ID)),
            ClientName = ReadNullableString(reader, ordinals, nameof(Client_GetResult.ClientName)),
            ClientNameBangla = ReadNullableString(reader, ordinals, nameof(Client_GetResult.ClientNameBangla)),
            ClientLastName = ReadNullableString(reader, ordinals, nameof(Client_GetResult.ClientLastName)),
            MobileNo = ReadNullableString(reader, ordinals, nameof(Client_GetResult.MobileNo)),
            EmailID = ReadNullableString(reader, ordinals, nameof(Client_GetResult.EmailID)),
            IsInstitute = ReadNullable<bool>(reader, ordinals, nameof(Client_GetResult.IsInstitute)),
            SubscriptionTypeID = ReadNullable<byte>(reader, ordinals, nameof(Client_GetResult.SubscriptionTypeID)),
            SubscriptionTypeEffectiveDate = ReadNullableDate(reader, ordinals, nameof(Client_GetResult.SubscriptionTypeEffectiveDate)),
            MemberNo = ReadNullable<int>(reader, ordinals, nameof(Client_GetResult.MemberNo)),
            IntroducerMemNo = ReadNullable<int>(reader, ordinals, nameof(Client_GetResult.IntroducerMemNo)),
            ClientStatusID = ReadNullable<byte>(reader, ordinals, nameof(Client_GetResult.ClientStatusID)),
            ClientStatusEffectiveDate = ReadNullableDate(reader, ordinals, nameof(Client_GetResult.ClientStatusEffectiveDate)),
            IDIssueDate = ReadNullableDate(reader, ordinals, nameof(Client_GetResult.IDIssueDate)),
            IsVerified = ReadRequired<bool>(reader, ordinals, nameof(Client_GetResult.IsVerified)),
            FatherName = ReadNullableString(reader, ordinals, nameof(Client_GetResult.FatherName)),
            FatherNameBang = ReadNullableString(reader, ordinals, nameof(Client_GetResult.FatherNameBang)),
            FatherClientNo = ReadNullable<int>(reader, ordinals, nameof(Client_GetResult.FatherClientNo)),
            MotherName = ReadNullableString(reader, ordinals, nameof(Client_GetResult.MotherName)),
            MotherNameBang = ReadNullableString(reader, ordinals, nameof(Client_GetResult.MotherNameBang)),
            MotherClientNo = ReadNullable<int>(reader, ordinals, nameof(Client_GetResult.MotherClientNo)),
            SpouseName = ReadNullableString(reader, ordinals, nameof(Client_GetResult.SpouseName)),
            SpouseNameBang = ReadNullableString(reader, ordinals, nameof(Client_GetResult.SpouseNameBang)),
            SpouseClientNo = ReadNullable<int>(reader, ordinals, nameof(Client_GetResult.SpouseClientNo)),
            ProfessionID = ReadNullable<byte>(reader, ordinals, nameof(Client_GetResult.ProfessionID)),
            Gender = ReadNullableString(reader, ordinals, nameof(Client_GetResult.Gender)),
            Religion = ReadNullableString(reader, ordinals, nameof(Client_GetResult.Religion)),
            MaritalStatus = ReadNullableString(reader, ordinals, nameof(Client_GetResult.MaritalStatus)),
            BloodGroup = ReadNullableString(reader, ordinals, nameof(Client_GetResult.BloodGroup)),
            DateOfBirth = ReadNullable<DateTime>(reader, ordinals, nameof(Client_GetResult.DateOfBirth)),
            PresentAddressID = ReadNullable<int>(reader, ordinals, nameof(Client_GetResult.PresentAddressID)),
            PermanentAddressID = ReadNullable<int>(reader, ordinals, nameof(Client_GetResult.PermanentAddressID)),
            EmployerName = ReadNullableString(reader, ordinals, nameof(Client_GetResult.EmployerName)),
            EmployerAddressID = ReadNullable<int>(reader, ordinals, nameof(Client_GetResult.EmployerAddressID)),
            NID = ReadNullableString(reader, ordinals, nameof(Client_GetResult.NID)),
            PassportNo = ReadNullableString(reader, ordinals, nameof(Client_GetResult.PassportNo)),
            TIN = ReadNullableString(reader, ordinals, nameof(Client_GetResult.TIN)),
            BirthPlaceDistrictID = ReadNullable<int>(reader, ordinals, nameof(Client_GetResult.BirthPlaceDistrictID)),
            EducationalQualification = ReadNullableString(reader, ordinals, nameof(Client_GetResult.EducationalQualification)),
            BirthCertificateNo = ReadNullableString(reader, ordinals, nameof(Client_GetResult.BirthCertificateNo)),
            GuardianClientIDIfAssociate = ReadNullable<int>(reader, ordinals, nameof(Client_GetResult.GuardianClientIDIfAssociate)),
            SignatoryName1 = ReadNullableString(reader, ordinals, nameof(Client_GetResult.SignatoryName1)),
            SignatoryName2 = ReadNullableString(reader, ordinals, nameof(Client_GetResult.SignatoryName2)),
            AddressID = ReadNullable<int>(reader, ordinals, nameof(Client_GetResult.AddressID)),
            TradelicenseNo = ReadNullableString(reader, ordinals, nameof(Client_GetResult.TradelicenseNo)),
            RegNo = ReadNullableString(reader, ordinals, nameof(Client_GetResult.RegNo))
        };
    }

    private static T ReadRequired<T>(
        SqlDataReader reader,
        IReadOnlyDictionary<string, int> ordinals,
        string columnName)
    {
        if (!ordinals.TryGetValue(columnName, out var ordinal) || reader.IsDBNull(ordinal))
        {
            throw new InvalidOperationException($"The [cl].[Client_Get] result is missing required column '{columnName}'.");
        }

        return reader.GetFieldValue<T>(ordinal);
    }

    private static T? ReadNullable<T>(
        SqlDataReader reader,
        IReadOnlyDictionary<string, int> ordinals,
        string columnName)
        where T : struct
    {
        return ordinals.TryGetValue(columnName, out var ordinal) && !reader.IsDBNull(ordinal)
            ? reader.GetFieldValue<T>(ordinal)
            : null;
    }

    private static string? ReadNullableString(
        SqlDataReader reader,
        IReadOnlyDictionary<string, int> ordinals,
        string columnName)
    {
        return ordinals.TryGetValue(columnName, out var ordinal) && !reader.IsDBNull(ordinal)
            ? reader.GetString(ordinal)
            : null;
    }

    private static DateOnly? ReadNullableDate(
        SqlDataReader reader,
        IReadOnlyDictionary<string, int> ordinals,
        string columnName)
    {
        return ordinals.TryGetValue(columnName, out var ordinal) && !reader.IsDBNull(ordinal)
            ? DateOnly.FromDateTime(reader.GetDateTime(ordinal))
            : null;
    }
}
