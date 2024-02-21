namespace Pango.Helpers;

public interface IComponentsPath
{
    public string Path { get; set; }
}

public record struct AllComponents(string Path) : IComponentsPath;

public record struct SingleComponent(string Path) : IComponentsPath;

public static class CommandsHelper
{
    public static IComponentsPath AllOrSingleComponent(string path)
    {
        if(path.Last() == '*')
        {
            path = path.Remove(path.Length - 1);
            return new AllComponents(path);
        }
        else
        {
            return new SingleComponent(path);    
        }
    }
}