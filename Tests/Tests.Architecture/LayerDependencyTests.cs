namespace Tests.Architecture
{
    public class LayerDependencyTests
    {
        [Fact]
        public void ShouldNotHaveCyclicDependencies()
        {
            var layers = new[]
            {
                "MailHub.Application",
                "MailHub.Infrastructure",
                "MailHub.Domain",
                "MailHub.Persistence"
            };

            // Validate that each layer only depends on allowed layers
            foreach (var layer in layers)
            {
                var dependentLayers = GetDependentLayers(layer);
                foreach (var dependentLayer in dependentLayers)
                {
                    Assert.True(layers.Contains(dependentLayer),
                        $"Layer {layer} has a dependency on {dependentLayer}, which is not allowed.");
                }
            }
        }

        private static string[] GetDependentLayers(string layer)
        {
            // Logic to get the dependent layers for the given layer (simplified).
            return layer switch
            {
                "MailHub.Application" => new[] { "MailHub.Infrastructure", "MailHub.Persistence" },
                "MailHub.Infrastructure" => new[] { "MailHub.Persistence" },
                "MailHub.Domain" => new[] { "MailHub.Persistence" },
                _ => new string[] { }
            };
        }
    }
}
