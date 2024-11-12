# Repository & Solution Overview

- [Repository & Solution Overview](#repository--solution-overview)
- [SnowboardShop.Api](#snowboardshopapi)
- [SnowboardShop.Application](#snowboardshopapplication)
- [SnowboardShop.Contracts](#snowboardshopcontracts)
- [SnowboardShop.Api.Tests.Integration](#snowboardshopapitestsintegration)
- [Helpers](#helpers)
  - [Identity.Api](#identity-api)
  - [Postman Collection & Swagger](#postman-collection--swagger)

This solution contains a demo for the test automation framework developed for the ``SnowboardShop.Api``, including multiple projects: the ``SnowboardShop.Api`` for managing snowboards and ratings, the ``SnowboardShop.Application`` layer for business logic and data management, ``SnowboardShop.Contracts`` for defining API request and response models, the ``SnwboardShop.Api.Tests.Integration`` to validate API functionalities, and Helper services like the ``Identity.Api`` for authentication.``Postman Collection`` and ``Swagger Documentation`` are available for manual debugging after running the database from ``docker-compose.yml`` and starting the ``SnowboardShop.Api``. 

More details about running the API Integration Tests, Docker setup, and the Postman Collection/Swagger are provided in the respective sections below. 

The repository pattern is a central piece of the solution. It is used to abstract data access logic from business logic and the API layer, keeping the database access code separate from both business logic and API logic. This separation makes the system more maintainable, testable, and flexible, as data access can be modified or replaced without affecting the rest of the application. The API layer interacts with the business logic through well-defined interfaces, ensuring that changes in the data access layer do not impact the API or business logic directly.

The test automation framework leverages several tools and technologies to create a robust and maintainable testing environment. The integration tests are built on ```.NET 8.0``` and use ``xUnit`` as the testing framework. ``Testcontainers`` is used to create and manage ``PostgreSQL`` and ``IdentityApi`` instances in isolated Docker containers for consistent and reliable test environments. ``Moq`` is used for mocking external dependencies to ensure tests remain isolated, while ``FluentAssertions`` provides a more expressive syntax for assertions, enhancing readability. ``Bogus`` is employed to generate realistic, fake test data, ensuring diversity in the test scenarios. Additionally, the ``WebApplicationFactory<TEntryPoint>.cs`` class from
``Microsoft.AspNetCore.Mvc.Testing`` allow us to run the web application in-memory for faster testing via the Web, and ``RestSharp`` facilitates creating and managing API requests during the tests. Configuration management is handled by ``DotNetEnv``, ensuring flexibility by loading environment variables from external files. ``Dapper`` is used for database interactions, providing a lightweight and efficient way to execute SQL queries and map the results to objects.

The ``Identity.Api`` contains a ``FakeVault`` for managing credentials and sensitive information within this demo framework. 

<u>This setup is **purely for demonstration purposes and should not be used in a production environment.**</u>  Replace these with secure storage and secret management practices for any deployment to a real environment.

<u>**Do not commit sensitive information** to public repositories or production environments without proper encryption or security measures!</u>
This setup is intended to help reviewers run and understand the demo framework locally without needing access to actual secrets management systems, but it is not a secure approach.

For more information, please refer to the additional README ``IMPORTANT_DemoSecrets.md`` located in the ``Identity.Api`` project, which provides further details about handling secrets in a secure manner.


### SnowboardShop.Api
- **Description:** This is the main Web API project that provides the core functionalities of the SnowboardShop, managing endpoints for snowboards and ratings, including CRUD operations.

  - Controllers, such as SnowboardsController and RatingsController, are thin and delegate business logic to the Application layer.
  - References the SnowboardShop.Application and SnowboardShop.Contracts projects.
  - Uses Dependency Injection to manage services and repositories, specifically by configuring dependency injection in the ``ApplicationServiceCollectionExtensions.cs`` class located in the ``SnowboardShop.Application`` project. This class registers services like ISnowboardRepository, IRatingService, and validators, allowing them to be injected into controllers and services without direct instantiation.
  - Implements ``Dapper`` for database interactions to maintain flexibility and control over database layers, allowing the database to be easily swapped in the future if needed. 
  - The ``IApiMarker`` interface is used as a marker interface to locate the API's entry point and is referenced in integration tests for configuring the WebApplicationFactory.

------------------------------------------------------------------------
### SnowboardShop.Application
- **Description:** Contains all business and infrastructure logic. Houses business rules, validation, and service implementation.  
The ``SnowboardShop.Application`` layer is responsible for all business logic, data access abstractions, and application-level services.  
This project encapsulates the core business functionalities without being aware of presentation concerns.

- Interaction with the database is achieved via repository classes like ``SnowboardRepository`` and ``RatingRepository``.
  - Service classes ``SnowboardService.cs/RatingService.cs`` implement business logic and call repository methods.
  - Services are registered using dependency injection abstractions without the application layer having a direct dependency on other services, which helps achieve separation of concerns.
  - The Validators are implemented using ``FluentValidation`` to validate the data models before they are processed by the service or repository.
  - Separation of concerns is maintained by dividing responsibilities between repositories (data access), services (business logic), and controllers (API endpoints).
- ``ApplicationServiceCollectionExtensions.cs`` handles dependency injection abstractions (IServiceCollection), allowing repositories, services, and validators to be injected where needed without tightly coupling the components. It is a central class for managing dependencies, ensuring that all services are configured and ready to use. This registration is included in the API project by calling ``builder.Services.AddApplication();`` within the ``Program.cs`` file.
------------------------------------------------------------------------
### SnowboardShop.Contracts

- **Description:** Contains the contracts for the API, including request and response models used across services.
  - Defines the data transfer objects (DTOs) that are used for communication between different layers of the application and with clients.
  - Holds data models for API requests and responses, providing a consistent contract for interaction.
  - Contracts use immutable models with init properties, ensuring that properties can be assigned only during object initialization, thereby preventing unintended modifications after creation. 
  - These contracts are shared across the different projects, ensuring consistency of the data structure across the solution.

------------------------------------------------------------------------
### SnowboardShop.Api.Tests.Integration

### Framework Overview

- **Description:** ``The SnowboardShop.Api.Tests.Integration`` project is the backbone of the ``SnowboardShopApi``'s testing strategy. It ensures that the various components of the system interact seamlessly, all while maintaining the same standards as in the production environment. The use of Testcontainers, the WebApplicationFactory, mocking, and dependency injection all contribute to making this a robust and reliable testing setup. This comprehensive approach to integration testing helps maintain the quality, reliability, and robustness of the ``SnowboardShopApi`` as it evolves.


**Core:**
  - **Factories:** 
    - The ``SnowboardsApiFactory`` class extends ``WebApplicationFactory<IApiMarker>`` and uses ``Testcontainers`` to automatically create isolated environments for each test, ensuring that all dependencies, such as ``PostgreSQL`` and the ``IdentityApi``, are spun up in their own containers. This automation ensures tests are executed in a consistent and controlled setup every time, reducing the chance of interference and increasing reliability. This approach is critical for ensuring test reliability by eliminating side effects between tests and simulating real-world conditions more accurately.   
    The ``IApiMarker.cs`` is a marker interface used to direct to a particular assembly, in this case, the ``SnowboardShop.Api``. This technique allows ``WebApplicationFactory`` to identify the appropriate API entry point without needing to specify concrete classes. Using a marker interface provides the flexibility of targeting the assembly generically, which makes the testing infrastructure more adaptable and decouples it from specific implementations.
    - ``DataSeedFactory``: There are two data seeding mechanisms: predefined database seed data packages and dynamic API-based data generation. The predefined seed packages are used to provide static, repeatable datasets that are shared across multiple tests, typically through direct database seeding. The ``DataSeedFactory.cs`` is responsible for managing this type of seeding. More details are available in the ``TestData`` section.
    - **Request Logging:** ``RequestLoggingDelegatingHandlerFactory.cs`` provides detailed logging for requests and responses during tests, aiding in debugging and analysis.
    - **IApiFactory:**  The ``IApiFactory`` interface provides a consistent way to create authenticated REST clients, ensuring secure and reliable interactions with the API during tests.
  - **Mock Providers:** 
    - Manage mocks for external dependencies, such as ``IUserContextService.cs``, allowing tests to simulate different user contexts without needing real implementations. By using MocksProvider, each dependency is registered and reset between tests, ensuring isolation and reproducibility of test scenarios. The ``MocksProvider.cs`` allows for easy customization, enabling or disabling specific mocks as required, providing flexibility in test scenarios. This enhances the ability to test under various conditions effectively, such as different user roles or invalid authentication states.
  - **Api Endpoints:** Centralizes API path definitions, ensuring consistency across all tests and making it easier to maintain endpoint changes.  

----

**Services:** The services layer provides utilities that facilitate common tasks across the integration tests, particularly focusing on authentication and token management.
  - **Api Authentication:** Manages the generation of access tokens and handles secure API access. This simulates real authentication scenarios, allowing integration tests to reflect realistic user interactions. 
  - **IdentityApi Integration:** Sets up and manages the ``IdentityApi`` used for validating and issuing tokens.

----
- **TestData:**.
The test data components provide a consistent and predictable test environment by establishing specific scenarios
  - **Data Seeding:** Handles data setup and teardown to ensure each test starts with a known state. There are two data seeding mechanisms available: **Database Seeding** & **API-based Seeding**. 
    - **Database Data Seeding:** Managed by ``DataSeedFactory`` and various seed packages, this mechanism is used to create consistent base states for testing. The database seeding provides static, repeatable datasets to ensure consistent testing conditions.
    ``IDataSeed.cs`` interface provides a standardized mechanism for implementing data seeding and clearing, allowing easy extension and reuse across multiple tests.
    - **API-Based Data Seeding:** This involves creating entities dynamically via API calls during tests, such as using ``SnowboardTestUtilities.CreateSnowboardAsync`` and ``CreateSnowboardFaker.cs``. Although dynamically generated, this approach follows defined rules, such as avoiding duplicate brand-year combinations, to ensure generated data is suitable for test scenarios. The generated data is not entirely random and adheres to rules specified by Faker to ensure consistency, including the use of ``SnowboardGenerationConstants.cs``
      - **TheoryData:** Theory data is used to test multiple variations of input scenarios systematically. It allows for parameterized tests, where different sets of data can be injected into the same test logic to validate the behavior of the system under diverse conditions. This helps ensure that the system performs correctly across a wide range of inputs and edge cases, without the need to write repetitive test code for each scenario.
      - **Json Data:** Predefined JSON files are used to work around restrictions of integration tests, which are executed together with backend code. Sometimes, using contracts directly does not allow modifications to initial properties, such as setting them to null or altering their values. These JSON files provide data for testing edge cases, such as missing fields or invalid formats, ensuring comprehensive validation of the system's behavior under different data inputs. This ensures comprehensive validation of the system's behavior under different data inputs.

  ----
- **Tests:** The integration tests are focused on ensuring the correctness and robustness of the SnowboardShop API by covering different scenarios:
  - **CRUD Operations:** Tests for create, read, update, and delete operations for snowboards and ratings, verifying that the API handles these operations as expected. 
  - **Data Validation:** Tests validate the system's resilience to invalid data inputs, ensuring that appropriate error handling is in place. 
  - **Authentication & Authorization:** Validates that only authorized users can access or modify data, using realistic access token management to test access control policies.
  - **Error Handling:** Verifies that the API responds appropriately to invalid or malformed requests, returning suitable error codes and messages.

- **TestUtilities:** The TestUtilities component provides reusable logic that simplifies and standardizes the implementation of tests:
  - **Assertions and Validation:** Provides reusable assertions to validate API responses against expected values, ensuring consistent and readable test validation logic. 
  - **Data Helpers:** Contains utilities for generating test data and performing common data manipulation tasks, reducing redundancy across tests. 
  - **Helper Methods:** Functions to streamline repetitive operations like creating or deleting entities, which improves readability and maintainability of the test code.

---------

### Helpers
#### Identity.Api

**<u>In a production environment, secrets, credentials, database connection strings, API keys, and any other sensitive information should never be hardcoded or stored in version control, but rather managed securely using environment variables or dedicated secret management services (e.g., AWS Secrets Manager, Azure Key Vault, Docker Secrets).</u>**

- **Description:** The ``Identity.Api`` provides an authentication service for generating JSON Web Tokens (JWTs) for users of the ``SnowboardShop.Api``. This is a supporting service for demonstration purposes, not intended for production use.
  - **IdentityController.cs:** This controller provides a /token endpoint, which accepts a request to generate an access token. The token generation uses a symmetric key (retrieved from the FakeVault), which means the same key is used for both signing and verifying the token. This is not recommended for production where asymmetric keys (e.g., public/private key pair) should be used for enhanced security.
  - **FakeVault:** Located in _FakeVault/Core/FakeVault.cs_, the ``FakeVault`` serves as a placeholder for storing credentials and other sensitive information in a controlled manner, suitable only for a non-production environment.
  - **DemoSecrets Folder:** Contains demo environment configuration and secrets, such as demo.env and demo_postgres_password.txt. These files help simplify the setup process for local use but must never be committed to version control to avoid compromising sensitive information.
  - **Dockerfile:** The Dockerfile within the ``Identity.Api`` project builds the application and generates a self-signed SSL certificate for local testing. This approach is not secure for production, where certificates from a trusted Certificate Authority should be used. 
  - **TokenGenerationRequest:** This class models the payload needed to generate a token, including properties like UserId, Email, and CustomClaims. The claims are included in the JWT for identity verification and are customizable based on the request.

----

#### Postman Collection & Swagger
- **Description:** Contains a Postman collection (_SnowboardShopApi.postman_collection.json_) for testing the ``SnowboardShop.Api`` endpoints manually, along with Swagger for exploring API documentation.
- **Prerequisites:** Docker & Postman installed
  - Navigate to the main solution ``SnowboardShop`` directory
  - Run ``docker-compose up``
  - Run ``SnowboardShop.Api`` from the IDE 
- **Testing in Postman:** Run the Postman collection located in _Helpers/PostmanCollection._
  - The Postman collection is automated, enabling efficient interaction with the API. 
  - To begin, generate a token from the ``Identity`` folder by using the ``POST TokenGeneration`` request in the collection. 
  - This step will automatically populate the authentication in the rest of the endpoints using collection variables. 
  - The data for each endpoint is also generated automatically using test scripts, which can be viewed in the _Scripts_ section within the Postman interface. 
- **Testing in Swagger:** Navigate to [Swagger](https://localhost:7001/swagger/index.html) to access the documentation and test the API endpoints (_by default it runs on port 7001_).


------------------------------------------------------------------------
------------------------------------------------------------------------
