using Aspire.Hosting.ApplicationModel;

namespace Aspire.Hosting;

public static class ProjectResourceImageTagExtensions
{
    /// <summary>
    /// Allows using WithImageTag on Aspire project resources.
    /// Internally maps to WithRemoteImageTag because ProjectResource is not a ContainerResource.
    /// </summary>
    public static IResourceBuilder<ProjectResource> WithImageTag(
        this IResourceBuilder<ProjectResource> builder,
        string imageTag)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(imageTag);

#pragma warning disable ASPIREPIPELINES003 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        return builder.WithRemoteImageTag(imageTag);
#pragma warning restore ASPIREPIPELINES003 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    }
}