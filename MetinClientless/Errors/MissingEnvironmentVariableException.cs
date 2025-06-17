namespace MetinClientless.Errors;

public class MissingEnvironmentVariableException: Exception
{
    public MissingEnvironmentVariableException(string variableName): base($"Environment variable {variableName} is missing.")
    {
        Console.WriteLine($"Environment variable {variableName} is missing.");
        Environment.Exit(1);
    }
}