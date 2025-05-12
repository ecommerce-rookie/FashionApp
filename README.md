# Fashion Store System

## Overview

Fashion Store is an advanced e-commerce platform designed to revolutionize the online shopping experience for fashion enthusiasts. This solution provides a comprehensive online shopping experience with modern architecture and best practices.

## Technologies Used

- **.NET 8.0**: Latest .NET platform for building the application
- **ASP.NET Core**: Web framework for building APIs and web applications
- **.NET Aspire**: Cloud-ready application stack for distributed applications
- **Entity Framework Core 9.0**: ORM for database interactions
- **PostgreSQL**: Primary database for data storage
- **Redis**: For distributed caching and session management
- **MediatR**: For implementing CQRS pattern
- **FluentValidation**: For robust validation rules
- **AutoMapper**: For object-to-object mapping
- **JWT Authentication**: For secure user authentication
- **SignalR**: For real-time communications
- **OpenTelemetry**: For observability and monitoring
- **Docker**: For containerization
- **Azure Cloud Services**: For deployment and hosting

## Features

- **User Management**: Registration, authentication, and profile management
- **Product Catalog**: Browsing, searching, and filtering products
- **Shopping Cart**: Adding, updating, and removing items
- **Order Processing**: Checkout, payment processing, and order history
- **Admin Dashboard**: Inventory management, order fulfillment, and analytics
- **Personalized Recommendations**: Based on user browsing and purchase history
- **Responsive Design**: Optimized for desktop, tablet, and mobile devices
- **Multi-language Support**: Internationalization for global customers
- **Payment Integration**: Multiple payment methods including credit cards and digital wallets

## Architecture

The application follows Clean Architecture principles with:

- **Domain Layer**: Core business entities and logic
- **Application Layer**: Business rules and use cases
- **Infrastructure Layer**: External services and data access
- **Presentation Layer**: API endpoints and user interfaces

### Design Patterns

- **CQRS Pattern**: Separating read and write operations
- **Repository Pattern**: Abstracting data access logic
- **Mediator Pattern**: Decoupling request handlers
- **Factory Pattern**: For creating complex objects
- **Strategy Pattern**: For interchangeable algorithms
- **Observer Pattern**: For event-driven components

## Project Structure

```
EcommerceApp/
├── src/
│   ├── apps/
│   │   ├── API/
│   │   │   ├── Controllers/
│   │   │   ├── Middlewares/
│   │   ├── Application/
│   │   │   ├── Dependencies/
│   │   │   ├── Attributes/
│   │   │   ├── Behaviors/
│   │   │   ├── Features/
│   │   │   ├── Mappings/
│   │   │   ├── Messages/
│   │   │   ├── Services/
│   │   │   ├── Utilities/
│   │   │   └── DependencyInjection.cs
│   │   ├── DOMAIN/
│   │   │   ├── Aggregates/
│   │   │   ├── Common/
│   │   │   ├── Constants/
│   │   │   ├── Enums/
│   │   │   ├── Exceptions/
│   │   │   ├── Models/
│   │   │   ├── Repositories/
│   │   │   ├── SeedWorks/
│   │   │   └── Shared/
│   │   ├── INFRASTRUCTURE/
│   │   │   ├── Authentication/
│   │   │   ├── BackgroundServices/
│   │   │   ├── Cache/
│   │   │   ├── Configurations/
│   │   │   ├── DocumentAPI/
│   │   │   ├── HttpClients/
│   │   │   ├── ProducerTasks/
│   │   │   ├── Shared/
│   │   │   ├── Storage/
│   │   │   ├── Versions/
│   │   │   └── DependencyInjection.cs
│   │   ├── PERSISTENCE/
│   │   │   ├── Configurations/
│   │   │   ├── Contexts/
│   │   │   ├── Extensions/
│   │   │   ├── Interceptors/
│   │   │   ├── Repository/
│   │   │   ├── UnitOfWork/
│   │   │   ├── AssemblyReference.cs
│   │   │   └── DependencyInjection.cs
│   ├── authen/
│   │   ├── IDENTITYSERVICE/
│   │   │   ├── Connected Services/
│   │   │   ├── Dependencies/
│   │   │   ├── Properties/
│   │   │   ├── wwwroot/
│   │   │   ├── Constants/
│   │   │   ├── Controllers/
│   │   │   ├── Data/
│   │   │   ├── Enums/
│   │   │   ├── Keys/
│   │   │   ├── Models/
│   │   │   ├── Pages/
│   │   │   ├── Utils/
│   │   │   ├── Config.cs
│   │   │   ├── CustomProfileService.cs
│   │   │   ├── CustomResourceOwnerPasswordValidator.cs
│   │   │   ├── HostingExtensions.cs
│   ├── hosts/
│   │   ├── AppHost/
│   │   │   ├── Program.cs
│   │   ├── ServiceDefaults/
│   │   │   ├── Extensions.cs
│   ├── presentations/
│   │   |── StoreFront/
│   │   │   ├── wwwroot/
│   │   │   ├── Application/
│   │   │   ├── Configurations/
│   │   │   ├── Domain/
│   │   │   ├── Pages/
│   │   │   ├── appsettings.json
│   │   │   ├──── Program.cs
├── test/
│   ├── Application.UnitTest/
│   │   ├── Application/
│   │   ├── CoverageReport/
│   │   ├── Domain/
│   │   ├── Infrastructure/
│   │   ├── Persistence/
│   │   ├── Extensions/
│   │   ├── Interceptors/
│   │   ├── Repositories/
│   │   ├── UnitOfWork/
│   │   ├── TestResults/
│   │   ├── coverlet.json
```

## Getting Started

### Prerequisites

- .NET 8.0 SDK or newer
- Docker Desktop
- PostgreSQL
- Redis

### Installation

1. Clone the repository
   ```bash
   git clone https://github.com/yourusername/EcommerceApp.git
   cd EcommerceApp
   ```

2. Set up the database
   ```bash
   docker-compose up -d postgres redis
   ```

3. Run the application
   ```bash
   cd src/EcommerceApp.AppHost
   dotnet run
   ```

4. Access the application:
   - API: https://localhost:5001
   - StoreFront: https://localhost:5002
   - Admin Dashboard: https://localhost:5003

### Running Tests

```bash
dotnet test
```

## Deployment

The application is configured for deployment to Azure using:

```bash
azd up
```

## CI/CD Pipeline

CI/CD is implemented through GitHub Actions with:
- Automated builds
- Unit and integration testing
- Code quality checks
- Containerization
- Deployment to staging/production environments

## Contributing

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details.