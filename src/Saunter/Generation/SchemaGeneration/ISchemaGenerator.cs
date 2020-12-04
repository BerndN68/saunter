using System;
using Saunter.AsyncApiSchema.v2;

namespace Saunter.Generation.SchemaGeneration
{
    [Obsolete("Saunter now uses NJsonSchema.Generation.JsonSchemaGenerator", true)]
    public interface ISchemaGenerator
    {
        /// <summary>
        /// Generate a schema, save it in the <paramref name="schemaRepository"/>, and return a reference to it.
        /// </summary>
        ISchema GenerateSchema(Type type, ISchemaRepository schemaRepository);
    }
}