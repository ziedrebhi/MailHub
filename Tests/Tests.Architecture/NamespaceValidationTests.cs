namespace Tests.Architecture
{
    public class NamespaceValidationTests
    {
        [Fact]
        public void ShouldNotDependOnApiLayer()
        {
            var allowedNamespaces = new[]
            {
                "MailHub.Application",
                "MailHub.Infrastructure",
                "MailHub.Domain",
                "MailHub.Persistence"
            };

            var disallowedNamespaces = new[]
            {
                "MailHub.Api"
            };

            var assemblies = new[]
            {
                typeof(MailHub.Application.EmailConfiguration.Commands.SetEmailConfigurationCommand).Assembly,
                typeof(MailHub.Infrastructure.Extensions.ServiceCollectionExtensions).Assembly,
                typeof(MailHub.Domain.Common.BaseEntity).Assembly,
                typeof(MailHub.Persistence.Extensions.ServiceCollectionExtensions).Assembly
            };

            // Get all types in the assemblies
            var types = assemblies.SelectMany(a => a.GetTypes()).ToList();

            foreach (var type in types)
            {
                if (disallowedNamespaces.Any(ns => type.Namespace?.StartsWith(ns) == true))
                {
                    var typeName = type.FullName ?? "Unknown";
                    Assert.True(allowedNamespaces.Any(ns => type.Namespace?.StartsWith(ns) == true),
                        $"Type '{typeName}' in a disallowed namespace.");
                }
            }
        }
    }
}
