
# 🏡 Property Rental Platform

Welcome to the **Property Rental Platform** – a full-stack solution designed to seamlessly connect **Landlords** 🧑‍💼 and **Tenants** 👨‍💻, with **Admins** 🛡️ managing the ecosystem. From listing properties to real-time chat and rental proposals, this system streamlines property rentals with ease and efficiency.

## 🌟 Features

### 👥 Actors
- 🛡️ **Admin**: Oversees the platform, verifies users, and moderates content.
- 🧑‍💼 **Landlord**: Manages property listings and handles rental applications.
- 👨‍💻 **Tenant**: Explores properties, submits proposals, and communicates in real time.

### 🔐 Authentication & Authorization
- Tenants can **browse properties without logging in**, but need to log in to apply or message.
- Admins **approve or reject** landlord registrations and property listings.
- JWT Authentication integrated with Swagger for secure and easy testing.

### 🏠 Property Listings
- Includes 📛 Landlord Name, 🏷️ Title, 📝 Description, 💰 Price, 📍 Location, 🖼️ Images, 👁️ View Count, and 📌 Rental Status.
- Advanced search: filter by **location**, **price**, and more.
- Realtime status updates using **SignalR**.

### 📄 Rental Proposals
- Tenants can **apply for rentals** with documents.
- Save favorite listings 💾.
- Landlords can **review, accept, or reject** proposals.
- A property becomes **unavailable** once rented.

### 💬 Real-Time Communication
- Built-in **chat system using SignalR** for direct messaging between tenants and landlords.
- Comment system for property discussions.

---

## 🧠 Architecture Overview

```
📁 Controllers         --> API endpoints
📁 DTOModels           --> Request/Response models
📁 Data
    ├── Models         --> Entity models
    └── DbContext      --> Database access (Scaffolded using Reverse Engineering)
📁 Extensions          --> JWT + Service registration helpers
📁 Hub                 --> SignalR hub for real-time chat 💬
📁 Repositories        --> Generic Repository pattern for data access
📁 Services            --> Business logic layer
📁 UOF (Unit of Work)  --> Transaction management layer
```

---

## 🛠️ Technologies Used

- **.NET Core** (Web API)
- **Entity Framework Core** with Scaffolded Reverse Engineering
- **SignalR** for real-time features
- **JWT** for secure authentication
- **Swagger** for API documentation and testing
- **SQL Server** for data persistence

---

## 🧩 Requirements

- ✅ Real-time communication via sockets (SignalR)
- ✅ Relational Database Design (DB Schema included)
- ✅ Secure authentication and role-based access
- ✅ Modular architecture with extensibility in mind

---

Feel free to fork, star, and contribute! ⭐  
Let's make renting simple and efficient! 💪
