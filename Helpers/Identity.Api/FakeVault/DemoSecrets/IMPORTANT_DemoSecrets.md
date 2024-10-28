# Demo Secret Management Information

## Purpose

This folder contains **Fake Vault & Demo Secrets** that are used purely for running the demo framework in a **non-production** environment. These secrets are hardcoded and stored in plaintext for the sake of simplicity, and should **never** be used in a real-world or production scenario.

This approach is for demonstration purposes only and is **not secure**. The setup is intended to help reviewers run and understand the demo framework locally without needing access to actual secrets management systems, but it is not a secure approach.

In a production environment, sensitive data must be managed securely using appropriate secret management solutions and must **never** be committed to version control.


## Security Considerations

- **These secrets are insecure by design** and are only meant for local or demo environments. They are stored in plaintext and are **not encrypted** or protected in any way.

- **Hardcoded Secrets Warning**  
  Storing sensitive data like usernames and passwords in plaintext and committing them to version control is highly insecure. In a real-world application, **never** commit sensitive information such as credentials, API keys, or connection strings to version control or store them in plaintext.

## Proper Practices for Secret Management

In real-world applications, secrets should be handled with care to avoid exposing sensitive information. Here are a few secure methods for managing secrets:

1. **Environment Variables**  
   Use environment variables to inject sensitive information at runtime. Make sure to avoid committing `.env` files that contain sensitive data to version control. You can exclude these files using `.gitignore`.

2. **User Secrets (For .NET Applications)**  
   When developing .NET applications locally, you can use **User Secrets** to store sensitive data securely during development. User Secrets are stored locally on your machine and are not committed to version control, providing an easy way to manage sensitive information without exposing it in your codebase.
This method is intended for **development** purposes only, not for production environments.

3. **Cloud-Based Secret Management Services**  
   Modern cloud platforms offer secure secret management services that handle encryption, access control, and auditing of secret access. Employ services like AWS Secrets Manager, Azure Key Vault, or Google Cloud Secret Manager to securely store and manage access to sensitive information.

4. **Docker Secrets**  
   Docker secrets provide a secure way to pass sensitive information (like passwords or certificates) to your containers at runtime. Secrets are managed as files inside the container, but Docker ensures they are not exposed in the environment.

5. **Encrypted Configuration Files**  
   For applications running on-premise or in a private cloud, you can store secrets in configuration files, but make sure they are **encrypted** at rest and decrypted at runtime.

## Docker Secrets Management 

In this demo framework, sensitive information is removed from the ``docker-compose.yml`` and ``Dockerfile`` to avoid hardcoding it into the codebase. Instead, it is placed into separate secret files and environment files for demonstration purposes. 

These files contain hardcoded sensitive data and must **never** be committed to version control in a real-world scenario. 
The idea is to replicate what can be done with real secure secret management solutions.

## FakeVault

In this demo framework, the **FakeVault** class is used to manage sensitive information in a controlled, non-production environment. The **FakeVault** simulates a vault or secure store within the application for storing credentials such as usernames, passwords, API keys, secrets, connection strings.

The **FakeVault** approach is purely for demonstration purposes, allowing the application to simulate the behavior of accessing secure credentials without relying on external secret management tools. This makes it easy to run the demo locally while emphasizing the need for secure practices in real-world scenarios.


- The ``FakeVault.cs`` class contains hardcoded values for sensitive information.
- These values are used within the application to simulate how secrets might be retrieved from a vault or secure store in a real-world setup.
- In production environments, the hardcoded values must be replaced with secure secret management solutions like environment variables/user secret/cloud-based secret management systems.

## DemoSecrets

The Demo Secrets folder contains hardcoded secrets and configuration files used exclusively for running the demo framework in a non-production environment. These secrets are provided to simplify the setup process and allow the application to simulate accessing secure credentials without the need for external secret management tools.


### Environment Variables

- The ``demo.env`` file is used to store environment variables required by the application. This file contains hardcoded sensitive information, such as the certificate password and database connection string.
- In a real-world scenario, this is insecure and must be avoided.
- In production, environment variables should be set securely in the deployment environment or managed through secure secret management tools.
- The demo.env file should be excluded from version control using **.gitignore**.

### Demo Secret Files

The demo secret files (``demo_postgres_user.txt/demo_postgres_password.txt``) contain hardcoded sensitive information used by the database service. The secret files are committed to version control in the demo for simplicity of running the demo framework.

- In a real-world scenario, sensitive information must not be committed to version control.
- In production, secrets should be managed securely using Docker secrets, environment variables, or external secret management systems.
- The secret files should be added to .gitignore to prevent accidental commits.

## Important Note

- These demo secrets are purely for development demo purposes and should not be used in production. Replace these with secure storage and secret management practices for any deployment to a real environment.
- **Do not commit sensitive information** to public repositories or production environments without proper encryption or security measures.
- This setup is intended to help reviewers run and understand the demo framework locally without needing access to actual secrets management systems, but it is not a secure approach.
