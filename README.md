
# Event Management System

FAI (Fashion AI Innovations) is an advanced e-commerce platform designed to revolutionize the online shopping experience for fashion enthusiasts. Built with ASP.NET and a clean architecture, FAI offers a seamless and personalized shopping experience, integrating AI technology to enhance product recommendations, image processing, and customer interactions.


## Demo

- [API Swagger]([https://fpt-event-management.azurewebsites.net/swagger/index.html](https://fashion-ai-innovations.azurewebsites.net/swagger/index.html))

## Features

- Product Catalog: Browse a wide range of clothing items with detailed descriptions, prices, and high-quality images.
- AI-Powered Recommendations: Get personalized product suggestions based on your browsing and purchase history.
- Shopping Cart & Checkout: Easily add items to your cart and complete your purchase with secure payment options.
- User Authentication: Secure login and registration for customers using JWT (JSON Web Tokens).
- Order Management: Track your orders, view order history, and receive real-time updates.
- Image Processing: Utilize AI for image enhancement and virtual try-ons, enhancing the shopping experience.
- Payment Integration: Seamless integration with MoMo and VnPay through PayOS for secure and reliable transactions.
- Real-Time Notifications: Stay updated with real-time notifications on order status and promotional offers via SignalR.
- Caching: Improve performance and load times with Redis caching.
- Background Services: Efficient processing of tasks such as email sending, image processing, and order handling with BackgroundService and RabbitMQ.

## Architecture

````
EventManagementSystem/
├── API/
│   ├── Controllers/
│   ├── Middleware/
│   ├── Models/
│   └── ...
├── Application/
│   ├── Abstractions/
│   ├── InternalServices/
│   ├── ExternalServices/
│   ├── Features/
│   ├── Helper/
│   ├── Messages/
│   └── ...
├── Domain/
│   ├── Entities/
│   ├── Models/
│   ├── Enums/
│   ├── Constants/
│   ├── Abstract/
│   └── ...
├── Infrastructure/
│   ├── Configuration/
│   ├── Extensions/
│   ├── Persistence/
│   ├── Repository/
│   └── .../
└── ...
````
## Installation & Setup

#### 1. Clone the Repository

- git clone '...'

- cd event-management-system

#### 2. Setup environment on appsettings.json

#### 3. Run the Application

- dotnet run

## Tech Stack

**Server:** asp.net

**Framework:** EF8

**Pattern:** CQRS Pattern, Mediator Pattern

**Technology:** FluentMail, FluentValidation, JWT Bear, Caching, RabbitMQ, Elasticsearch, MediaTR, BackgroundTask

**Database:** Postgre Server, Redis

## Authors

- [Le Huy](https://github.com/MaxH2k3)
- [Chung Quy](https://github.com/quy-ndc)
- [Phuc An](https://github.com/wave-npa)