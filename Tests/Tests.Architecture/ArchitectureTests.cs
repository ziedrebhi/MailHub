namespace Tests.Architecture
{
    public class ArchitectureTests
    {
        [Fact]
        public void ShouldHaveValidLayeredArchitecture()
        {
            // Call specific tests
            var layerDependencyTests = new LayerDependencyTests();
            layerDependencyTests.ShouldNotHaveCyclicDependencies();

            var namespaceValidationTests = new NamespaceValidationTests();
            namespaceValidationTests.ShouldNotDependOnApiLayer();

            var serviceRegistrationTests = new ServiceRegistrationTests();
            serviceRegistrationTests.ShouldRegisterServicesInCorrectLayers();
        }
    }
}
