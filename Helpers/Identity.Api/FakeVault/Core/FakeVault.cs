namespace Identity.Api.FakeVault.Core;

/*
 * FakeVault serves as a placeholder for managing credentials and sensitive information
 * within this demo framework. It allows for hardcoded values to be used in a controlled,
 * non-production environment to simulate how secrets would be managed in a real-world
 * scenario.
 *
 * This class is not secure and should never be used in production. In actual deployments,
 * credentials and sensitive data should be managed securely using environment variables,
 * secrets management tools (e.g., Docker secrets, AWS Secrets Manager, Azure Key Vault), or
 * encrypted configuration files.
 *
 * IMPORTANT: Never commit sensitive information to version control.
 * In a real environment, secrets should be excluded from repositories using gitignore or
 * similar mechanisms, and stored securely.
 *
 * This demo vault may contain other credentials for different services as needed.
 * Replace this with secure practices before deploying to any production environment.
 */

public static class FakeVault
{
    public const string DatabaseName = "snowboards";
    public const string DatabaseUsername = "demouser";
    public const string DatabasePassword = "changeme";
    
    public const string TokenSecret = "DemoPurposesOnlyThisMustBeStoredSecurelyInTheRealWorldForExampleInUserSecretOrEnvironmentVariableOrKeyVault";
}