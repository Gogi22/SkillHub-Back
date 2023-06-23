using System.Text.Json.Serialization;

namespace Application.Entities;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Role
{
    Freelancer,
    Client,
}