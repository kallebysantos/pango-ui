using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Pango.Abstractions;

namespace DGAV.Candidatos.Shared.Converters;

public class OptionJsonConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
        => typeToConvert.AssemblyQualifiedName is not null
            && typeToConvert.AssemblyQualifiedName.Contains(typeof(Option).ToString());
    

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        => Option.From(typeToConvert.GetGenericArguments().FirstOrDefault())
            .MapOrDefault(some => (JsonConverter)Activator.CreateInstance(
                type: typeof(InnerOptionJsonConverter<>).MakeGenericType([some]),
                bindingAttr: BindingFlags.Instance | BindingFlags.Public,
                binder: null, 
                args: [options],
                culture: null
            )!);

    private class InnerOptionJsonConverter<T>(JsonSerializerOptions options) : JsonConverter<Option<T>>
    {
        public override bool HandleNull => true;
        private readonly JsonConverter<T> valueConverter = (JsonConverter<T>)options.GetConverter(typeof(T));

        public override Option<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => reader.TokenType is JsonTokenType.Null
                ? Option.None<T>()
                : Option.Some(valueConverter.Read(ref reader, typeToConvert, options)!);

        public override void Write(Utf8JsonWriter writer, Option<T> value, JsonSerializerOptions options)
        {
            value.Match(
                some: v => valueConverter.Write(writer, v, options),
                none: writer.WriteNullValue
            );
        }
    }
}