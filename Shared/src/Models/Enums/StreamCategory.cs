using System.Text.Json.Serialization;
using System.Runtime.Serialization;

namespace Shared.models.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum StreamCategory
{
    [EnumMember(Value = "Gaming")]
    Gaming,
    
    [EnumMember(Value = "Music")]
    Music,
    
    [EnumMember(Value = "Sports")]
    Sports,
    
    [EnumMember(Value = "Art")]
    Art,
    
    [EnumMember(Value = "Cooking")]
    Cooking,
    
    [EnumMember(Value = "Education")]
    Education,
    
    [EnumMember(Value = "Travel")]
    Travel,
    
    [EnumMember(Value = "TalkShows")]
    TalkShows,
    
    [EnumMember(Value = "News")]
    News,
    
    [EnumMember(Value = "Technology")]
    Technology,
    
    [EnumMember(Value = "Science")]
    Science,
    
    [EnumMember(Value = "HealthAndFitness")]
    HealthAndFitness,
    
    [EnumMember(Value = "FashionAndBeauty")]
    FashionAndBeauty,
    
    [EnumMember(Value = "FoodAndDrink")]
    FoodAndDrink,
    
    [EnumMember(Value = "PetsAndAnimals")]
    PetsAndAnimals,
    
    [EnumMember(Value = "DiyAndCrafts")]
    DiyAndCrafts
}