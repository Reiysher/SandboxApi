using SandboxApi.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SandboxApi.Converters;

public class FieldJsonConverter : JsonConverter<FieldDto>
{
    enum TypeDiscriminator
    {
        String = 1,
        Number = 2,
        DateTime = 3
    }

    public override bool CanConvert(Type typeToConvert) =>
        typeof(FieldDto).IsAssignableFrom(typeToConvert);

    public override FieldDto Read(
        ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        reader.Read();
        if (reader.TokenType != JsonTokenType.PropertyName)
        {
            throw new JsonException();
        }

        string? propertyName = reader.GetString();
        if (propertyName != "Type")
        {
            throw new JsonException();
        }

        reader.Read();
        if (reader.TokenType != JsonTokenType.Number)
        {
            throw new JsonException();
        }

        TypeDiscriminator typeDiscriminator = Enum.Parse<TypeDiscriminator>(reader.GetString()!);
        FieldDto field = typeDiscriminator switch
        {
            TypeDiscriminator.String => new StringFieldDto(),
            TypeDiscriminator.Number => new NumberFieldDto(),
            TypeDiscriminator.DateTime => new DateFieldDto(),
            _ => throw new JsonException()
        };

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return field;
            }

            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                propertyName = reader.GetString();
                reader.Read();
                switch (propertyName)
                {
                    case "Value":
                        switch (typeDiscriminator)
                        {
                            case TypeDiscriminator.String:
                                string? stringValue = reader.GetString();
                                ((StringFieldDto)field).Value = stringValue;
                                break;
                            case TypeDiscriminator.Number:
                                int numberValue = reader.GetInt32();
                                ((NumberFieldDto)field).Value = numberValue;
                                break;
                            case TypeDiscriminator.DateTime:
                                DateTime dateTimeValue = reader.GetDateTime();
                                ((DateFieldDto)field).Value = dateTimeValue;
                                break;
                        }
                        break;
                    case "Name":
                        string? name = reader.GetString();
                        field.Name = name;
                        break;
                    case "Description":
                        string? description = reader.GetString();
                        field.Description = description;
                        break;
                }
            }
        }
        throw new JsonException();
    }

    public override void Write(
        Utf8JsonWriter writer, FieldDto field, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WriteString("Name", field.Name);
        writer.WriteString("Description", field.Description);

        if (field is StringFieldDto stringField)
        {
            writer.WriteString("Type", TypeDiscriminator.String.ToString());
            writer.WriteString("Value", stringField.Value);
        }
        else if (field is NumberFieldDto numberField)
        {
            writer.WriteString("Type", TypeDiscriminator.Number.ToString());
            writer.WriteNumber("Value", numberField.Value);
        }
        else if (field is DateFieldDto dateField)
        {
            writer.WriteString("Type", TypeDiscriminator.DateTime.ToString());
            writer.WriteString("Value", dateField.Value);
        }

        writer.WriteEndObject();
    }
}