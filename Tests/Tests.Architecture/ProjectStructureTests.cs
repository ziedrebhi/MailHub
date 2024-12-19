namespace Tests.Architecture
{
    public class ProjectStructureTests
    {
        [Fact]
        public void ShouldFollowCleanArchitectureStructure()
        {
            var solutionDirectory = GetSolutionDirectory();
            var srcDirectory = Path.Combine(solutionDirectory, "src");

            var expectedFolders = new[]
            {
        "MailHub.Application",
        "MailHub.Infrastructure",
        "MailHub.Api",
        "MailHub.Domain",
        "MailHub.Persistence"
    };

            foreach (var folder in expectedFolders)
            {
                var folderPath = Path.Combine(srcDirectory, folder);
                Assert.True(Directory.Exists(folderPath), $"Expected folder '{folder}' not found in 'src'.");
            }
        }

        private static string GetSolutionDirectory()
        {
            var currentDirectory = Directory.GetCurrentDirectory();

            // Traverse up until the .sln file is found
            while (!Directory.GetFiles(currentDirectory, "*.sln").Any())
            {
                currentDirectory = Directory.GetParent(currentDirectory)?.FullName;

                if (currentDirectory == null)
                {
                    throw new DirectoryNotFoundException("Solution directory not found.");
                }
            }

            return currentDirectory;
        }

    }
}
