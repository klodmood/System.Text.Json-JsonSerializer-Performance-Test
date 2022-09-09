using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using JsonSerializerOptions = System.Text.Json.JsonSerializerOptions;

namespace Seahorse.UnitTests;

[MemoryDiagnoser]
public class Program
{
    public static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run<Program>(ManualConfig.Create(DefaultConfig.Instance).WithOptions(ConfigOptions.DisableOptimizationsValidator));
    }
    
    public static CustomData data = new CustomData()
    {
        Address1 = "Address1",
        Address2 = "Address2",
        Address3 = "Address3",
        AnotherName = "AnotherName",
        City = "Portsmouth",
        ContactEmail = "a@b.com",
        ContactName = "ContactName",
        Closed = new DateOnly(2022, 1, 1),
        Opened = new DateOnly(2022, 1, 1),
        Country = "fdfdsaf",
        IntegerVal1 = 420,
        IntegerVal2 = 20,
        Id = "Id1",
        AnotherId = "Id2",
        PostCode = "AB12CD",
        Name = "Name",
        AnotherClass = new AnotherClass
        {
            Nested1 = "NestedStringData",
            NestedDate = new DateTime(2022,1,1)
        }
    };
    
    [Benchmark]
    public void TestWithStaticMethod()
    {
        var testData = new CustomData();
        for (int i = 0; i < 10000; i++)
        {
            var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data, GetJsonSerializerOptions()));
            testData = JsonSerializer.Deserialize<CustomData>(bytes, GetJsonSerializerOptions());
        }
    }

    
    [Benchmark]
    public void TestWithStaticVariable()
    {
        var testData = new CustomData();
        for (int i = 0; i < 10000; i++)
        {
            var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data, _serializerOptions));
            testData = JsonSerializer.Deserialize<CustomData>(bytes, _serializerOptions);
        }

    }

    
    private static JsonSerializerOptions _serializerOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = null,
        WriteIndented = true,
        AllowTrailingCommas = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new DateOnlyConverter() }
    };

    private static JsonSerializerOptions GetJsonSerializerOptions()
    {
        return new JsonSerializerOptions
        {
            PropertyNamingPolicy = null,
            WriteIndented = true,
            AllowTrailingCommas = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters = { new DateOnlyConverter() }
        };
    }
}
public class DateOnlyConverter : JsonConverter<DateOnly>
{
    private readonly string serializationFormat;

    public DateOnlyConverter() : this(null)
    {
    }

    public DateOnlyConverter(string? serializationFormat)
    {
        this.serializationFormat = serializationFormat ?? "yyyy-MM-dd";
    }

    public override DateOnly Read(ref Utf8JsonReader reader, 
        Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        return DateOnly.Parse(value!);
    }

    public override void Write(Utf8JsonWriter writer, DateOnly value, 
        JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString(serializationFormat));
}

public class CustomData
{
    public string Id { get; set; } = string.Empty;
    
    public string AnotherId { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public decimal DecimalValue { get; set; }
    
    public string ContactName { get; set; } = string.Empty;

    public string ContactEmail { get; set; } = string.Empty;

    public string AnotherName { get; set; } = string.Empty;
    public string Address1 { get; set; } = string.Empty;
    public string Address2 { get; set; } = string.Empty;
    public string Address3 { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string PostCode { get; set; } = string.Empty;

    public int IntegerVal1 { get; set; }

    public int IntegerVal2 { get; set; }

    public DateOnly Opened { get; set; }
    public DateOnly? Closed { get; set; }

    public AnotherClass AnotherClass { get; set; }
}

public class AnotherClass
{
    public string Nested1 { get; set; }
    public DateTime NestedDate { get; set; }
}